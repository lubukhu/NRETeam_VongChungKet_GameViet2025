using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Core Stats")]
    [Range(0, 100)] public int finance = 50;
    [Range(0, 100)] public int trust = 50;
    [Range(0, 100)] public int environment = 50;
    [Range(0, 100)] public int culture = 50;

    [Header("Game References")]
    public Instantiator cardInstantiator;
    public List<CardData> allAvailableCards;
    public CardData startingCard;
    public GameEndingsConfig gameEndingsConfig;

    [Header("Game State")]
    public List<CardData> availableDeck;
    public List<CardData> playedCards;
    public CardData deathCard;

    public event Action<StatType, int> OnStatChanged;
    public event Action<string> OnGameEnded;

    private CardData _currentCardData;
    private CardData _nextCardDataToLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (cardInstantiator != null)
        {
            cardInstantiator.OnActiveCardDisplayReady += HandleActiveCardDisplayReady;
        }
        else
        {
            Debug.LogError("GameManager: Card Instantiator not assigned in OnEnable! Please assign it in Inspector.", this);
        }
    }

    void OnDisable()
    {
        if (cardInstantiator != null)
        {
            cardInstantiator.OnActiveCardDisplayReady -= HandleActiveCardDisplayReady;
        }
    }

    void Start()
    {
        if (cardInstantiator == null)
        {
            Debug.LogError("GameManager: Card Instantiator is not assigned!", this);
            return;
        }
        if (gameEndingsConfig == null)
        {
            Debug.LogError("GameManager: Game Endings Config is not assigned!", this);
            return;
        }
        if (startingCard == null)
        {
            Debug.LogError("GameManager: Starting Card is not assigned!", this);
            return;
        }

        InitializeDeck();

        _currentCardData = startingCard;
        cardInstantiator.CreateInitialCards(startingCard);
    }

    private void InitializeDeck()
    {
        availableDeck = new List<CardData>(allAvailableCards);
        if (startingCard != null && availableDeck.Contains(startingCard))
        {
            availableDeck.Remove(startingCard);
        }
        ShuffleDeck(availableDeck);
    }

    private void ShuffleDeck(List<CardData> deck)
    {
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardData value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    public void HandleCardSwipe(bool isRightSwipe)
    {
        if (_currentCardData == null)
        {
            Debug.LogWarning("GameManager: No current card data to process swipe!", this);
            return;
        }

        List<CardEffect> effectsToApply = isRightSwipe ? _currentCardData.rightChoiceEffects : _currentCardData.leftChoiceEffects;
        string nextCardIDFromChoice = isRightSwipe ? _currentCardData.nextCardIDAfterRightChoice : _currentCardData.nextCardIDAfterLeftChoice;

        ApplyCardEffects(effectsToApply);

        playedCards.Add(_currentCardData);

        if (CheckForGameEndCondition())
        {
            return;
        }

        _nextCardDataToLoad = GetNextCard(nextCardIDFromChoice);

        if (_nextCardDataToLoad != null)
        {
            Debug.Log($"GameManager: Next card chosen (waiting for display to be ready): {_nextCardDataToLoad.characterName}");
        }
        else
        {
            Debug.LogError("GameManager: Could not get a valid next card. This might lead to an empty card display.", this);
            OnGameEnded?.Invoke(gameEndingsConfig.GetEndingMessage("NoMoreCards"));
            Time.timeScale = 0;
        }
    }

    private void HandleActiveCardDisplayReady(CardDisplay readyCardDisplay)
    {
        Debug.Log("GameManager: Received OnActiveCardDisplayReady event.");
        if (_nextCardDataToLoad != null)
        {
            readyCardDisplay.SetupCard(_nextCardDataToLoad);
            _currentCardData = _nextCardDataToLoad;
            _nextCardDataToLoad = null;
            Debug.Log($"GameManager: Displayed new card: {_currentCardData.characterName}");
        }
        else if (startingCard != null && readyCardDisplay.GetCurrentCardData() == startingCard)
        {
            Debug.Log("GameManager: Initial card display ready (data already set).");
        }
        else
        {
            Debug.LogWarning("GameManager: OnActiveCardDisplayReady called but no next card data was queued or initial card not recognized. Card might be empty.", this);
        }
    }

    private CardData GetNextCard(string nextCardIDFromChoice)
    {
        CardData chosenCard = null;

        if (!string.IsNullOrEmpty(nextCardIDFromChoice))
        {
            CardData targetedCard = allAvailableCards.FirstOrDefault(card => card.cardID == nextCardIDFromChoice);

            if (targetedCard != null)
            {
                if (deathCard != null && targetedCard.cardID == deathCard.cardID)
                {
                    Debug.Log($"GameManager: Targeted Death Card '{nextCardIDFromChoice}' selected.");
                    if (availableDeck.Contains(targetedCard)) availableDeck.Remove(targetedCard);
                    return targetedCard;
                }

                if (!playedCards.Contains(targetedCard))
                {
                    chosenCard = targetedCard;
                    if (availableDeck.Contains(chosenCard))
                    {
                        availableDeck.Remove(chosenCard);
                    }
                    Debug.Log($"GameManager: Targeted card '{nextCardIDFromChoice}' selected.");
                    return chosenCard;
                }
                else
                {
                    Debug.LogWarning($"GameManager: Targeted card '{nextCardIDFromChoice}' already played. Choosing random instead.", this);
                }
            }
            else
            {
                Debug.LogWarning($"GameManager: Targeted card '{nextCardIDFromChoice}' not found in allAvailableCards. Choosing random instead.", this);
            }
        }

        if (availableDeck.Count > 0)
        {
            chosenCard = availableDeck[0];
            availableDeck.RemoveAt(0);
            Debug.Log($"GameManager: Random card '{chosenCard.cardID}' selected.");
            return chosenCard;
        }
        else
        {
            Debug.LogWarning("GameManager: All available cards have been played. No more random cards.", this);
            return null;
        }
    }

    private void ApplyCardEffects(List<CardEffect> effects)
    {
        if (effects == null) return;

        foreach (var effect in effects)
        {
            int currentValue = 0;
            switch (effect.statType)
            {
                case StatType.Finance:
                    finance = Mathf.Clamp(finance + effect.changeAmount, 0, 100);
                    currentValue = finance;
                    break;
                case StatType.Trust:
                    trust = Mathf.Clamp(trust + effect.changeAmount, 0, 100);
                    currentValue = trust;
                    break;
                case StatType.Environment:
                    environment = Mathf.Clamp(environment + effect.changeAmount, 0, 100);
                    currentValue = environment;
                    break;
                case StatType.Culture:
                    culture = Mathf.Clamp(culture + effect.changeAmount, 0, 100);
                    currentValue = culture;
                    break;
                default:
                    Debug.LogWarning($"GameManager: Unhandled StatType: {effect.statType}", this);
                    break;
            }
            OnStatChanged?.Invoke(effect.statType, currentValue);
        }
    }

    private bool CheckForGameEndCondition()
    {
        string endingId = "";
        if (finance <= 0) endingId = "FinanceZero";
        else if (finance >= 100) endingId = "FinanceHundred";
        else if (trust <= 0) endingId = "TrustZero";
        else if (trust >= 100) endingId = "TrustHundred";
        else if (environment <= 0) endingId = "EnvironmentZero";
        else if (environment >= 100) endingId = "EnvironmentHundred";
        else if (culture <= 0) endingId = "CultureZero";
        else if (culture >= 100) endingId = "CultureHundred";

        if (!string.IsNullOrEmpty(endingId))
        {
            string endingMessage = gameEndingsConfig.GetEndingMessage(endingId);
            OnGameEnded?.Invoke(endingMessage);
            Debug.Log($"Game Over! Ending: {endingMessage}");
            Time.timeScale = 0;
            return true;
        }
        return false;
    }
}