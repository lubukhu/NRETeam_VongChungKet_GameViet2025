
// [tooltips] "Bộ não" kể chuyện, áp dụng logic xác suất để tạo ra các pool thẻ bài phù hợp với trạng thái game.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;
using System.Linq;

public class NarrativeDirector : MonoBehaviour
{
    [Header("Game State Inputs")]
    [SerializeField] private FloatVariable financeStat;
    [SerializeField] private FloatVariable trustStat;
    [SerializeField] private FloatVariable environmentStat;
    [SerializeField] private FloatVariable cultureStat;

    [Header("Master Decks")]
    [SerializeField] private ScriptableListCardData financeDeck;
    [SerializeField] private ScriptableListCardData trustDeck;
    [SerializeField] private ScriptableListCardData environmentDeck;
    [SerializeField] private ScriptableListCardData cultureDeck;
    [SerializeField] private ScriptableListCardData genericDeck;
    
    [Header("Runtime Data")]
    [SerializeField] private ScriptableListCardData playedCardsThisRun;
    [SerializeField] private ScriptableListCardData currentCardPool;
    
    [Header("Output Events")]
    [SerializeField] private ScriptableEventNoParam onNewPoolReady;

    [Header("Config")]
    [SerializeField] private int poolSize = 5;

    public void GenerateNextPool()
    {
        var masterDecks = new Dictionary<StatType, ScriptableListCardData>
        {
            { StatType.Finance, financeDeck },
            { StatType.Trust, trustDeck },
            { StatType.Environment, environmentDeck },
            { StatType.Culture, cultureDeck }
        };

        var weights = CalculateWeights();
        List<CardData> nextPool = new List<CardData>();
        
        while (nextPool.Count < poolSize)
        {
            var availableCardsFromAllDecks = masterDecks.Values
                .Where(deck => deck != null)
                .SelectMany(deck => deck.ToList())
                .Where(card => !playedCardsThisRun.Contains(card) && !nextPool.Contains(card))
                .ToList();

            if (availableCardsFromAllDecks.Count == 0) break;

            StatType chosenCategory = GetRandomCategoryByWeight(weights);
            
            var potentialCards = masterDecks.ContainsKey(chosenCategory) && masterDecks[chosenCategory] != null
                ? masterDecks[chosenCategory]
                    .Where(c => !playedCardsThisRun.Contains(c) && !nextPool.Contains(c))
                    .ToList()
                : new List<CardData>();

            if (potentialCards.Count > 0)
            {
                int randomIndex = Random.Range(0, potentialCards.Count);
                nextPool.Add(potentialCards[randomIndex]);
            }
        }
        
        currentCardPool.AddRange(nextPool);
        onNewPoolReady.Raise();
    }

    private Dictionary<StatType, float> CalculateWeights()
    {
        var weights = new Dictionary<StatType, float>();
        weights[StatType.Finance] = CalculateWeightForStat(financeStat.Value);
        weights[StatType.Trust] = CalculateWeightForStat(trustStat.Value);
        weights[StatType.Environment] = CalculateWeightForStat(environmentStat.Value);
        weights[StatType.Culture] = CalculateWeightForStat(cultureStat.Value);
        return weights;
    }

    private float CalculateWeightForStat(float statValue)
    {
        float distanceFromCenter = Mathf.Abs(50f - statValue);
        return 1f + (distanceFromCenter / 50f);
    }

    private StatType GetRandomCategoryByWeight(Dictionary<StatType, float> weights)
    {
        float totalWeight = weights.Values.Sum();
        if (totalWeight <= 0) return weights.Keys.First();
        
        float randomPoint = Random.Range(0, totalWeight);

        foreach (var pair in weights)
        {
            if (randomPoint < pair.Value)
                return pair.Key;
            randomPoint -= pair.Value;
        }
        return weights.Keys.First();
    }
}