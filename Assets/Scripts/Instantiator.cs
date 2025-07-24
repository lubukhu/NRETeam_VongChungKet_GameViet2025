using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Instantiator : MonoBehaviour
{
    public GameObject cardPrefab;
    private SwipeEffect _currentActiveSwipeEffect;
    private CardDisplay _currentActiveCardDisplay;

    public event Action<CardDisplay> OnActiveCardDisplayReady;

    void OnEnable()
    {
        SecondCard.OnSecondCardBecomesActiveSwipeEffect += HandleNewActiveSwipeEffect;
    }

    void OnDisable()
    {
        SecondCard.OnSecondCardBecomesActiveSwipeEffect -= HandleNewActiveSwipeEffect;
        if (_currentActiveSwipeEffect != null)
        {
            _currentActiveSwipeEffect.cardDestroyed -= OnCardDestroyed;
            if (GameManager.Instance != null)
            {
                _currentActiveSwipeEffect.cardSwiped -= GameManager.Instance.HandleCardSwipe;
            }
        }
    }

    void Start()
    {
        // GameManager sẽ gọi CreateInitialCards để bắt đầu game
    }

    public void CreateInitialCards(CardData initialCardData)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 1. Sinh ra thẻ đầu tiên (thẻ trên cùng) - Thẻ này sẽ là thẻ hoạt động chính, có SwipeEffect.
        GameObject firstCardObject = Instantiate(cardPrefab, transform, false);
        firstCardObject.transform.SetAsLastSibling();

        SwipeEffect initialSwipeEffect = firstCardObject.GetComponent<SwipeEffect>();
        if (initialSwipeEffect == null) initialSwipeEffect = firstCardObject.AddComponent<SwipeEffect>();
        SecondCard firstCardSecondCard = firstCardObject.GetComponent<SecondCard>();
        if (firstCardSecondCard != null) Destroy(firstCardSecondCard);

        _currentActiveCardDisplay = firstCardObject.GetComponent<CardDisplay>();
        if (_currentActiveCardDisplay == null) _currentActiveCardDisplay = firstCardObject.AddComponent<CardDisplay>();
        _currentActiveCardDisplay.SetupCard(initialCardData);


        // 2. Sinh ra thẻ thứ hai (thẻ phía sau) - Thẻ này sẽ có SecondCard.
        GameObject secondCardObject = Instantiate(cardPrefab, transform, false);
        secondCardObject.transform.SetAsFirstSibling();
        SecondCard secondCardComponent = secondCardObject.GetComponent<SecondCard>();
        if (secondCardComponent == null) secondCardComponent = secondCardObject.AddComponent<SecondCard>();
        SwipeEffect secondCardSwipeEffect = secondCardObject.GetComponent<SwipeEffect>();
        if (secondCardSwipeEffect != null) Destroy(secondCardSwipeEffect);

        if (secondCardObject.GetComponent<CardDisplay>() == null)
        {
            secondCardObject.AddComponent<CardDisplay>();
        }

        // Đảm bảo thẻ SECOND CARD MỚI SINH RA HIỂN THỊ MẶT SAU (khi được tạo trong CreateInitialCards)
        // LƯU Ý: Ở phiên bản code này, SecondCard chưa có cardFront/cardBack.
        // Bạn sẽ cần chỉnh sửa SecondCard và Instantiator để xử lý mặt sau nếu muốn hiệu ứng lật.
        // if (secondCardComponent != null && secondCardComponent.cardBack != null && secondCardComponent.cardFront != null)
        // {
        //     secondCardComponent.cardBack.SetActive(true);
        //     secondCardComponent.cardFront.SetActive(false);
        // }

        if (secondCardComponent != null)
        {
            secondCardComponent.SetFirstCardToObserve(firstCardObject, initialSwipeEffect);
        }
        else
        {
            Debug.LogError("Instantiator: Second card prefab is missing SecondCard component or failed to add!", this);
        }

        HandleNewActiveSwipeEffect(initialSwipeEffect);
    }

    private GameObject InstantiateNewCard_ForNextSecond()
    {
        GameObject newCard = Instantiate(cardPrefab, transform, false);
        newCard.transform.SetAsFirstSibling();

        SecondCard sc = newCard.GetComponent<SecondCard>();
        if (sc == null) sc = newCard.AddComponent<SecondCard>();
        SwipeEffect se = newCard.GetComponent<SwipeEffect>();
        if (se != null) Destroy(se);

        if (newCard.GetComponent<CardDisplay>() == null)
        {
            newCard.AddComponent<CardDisplay>();
        }

        // Đảm bảo thẻ SECOND CARD MỚI SINH RA HIỂN THỊ MẶT SAU
        // LƯU Ý: Ở phiên bản code này, SecondCard chưa có cardFront/cardBack.
        // if (sc != null && sc.cardBack != null && sc.cardFront != null)
        // {
        //     sc.cardBack.SetActive(true);
        //     sc.cardFront.SetActive(false);
        // }

        return newCard;
    }


    private void HandleNewActiveSwipeEffect(SwipeEffect newActiveSwipeEffect)
    {
        if (_currentActiveSwipeEffect != null)
        {
            _currentActiveSwipeEffect.cardDestroyed -= OnCardDestroyed;
            if (GameManager.Instance != null)
            {
                _currentActiveSwipeEffect.cardSwiped -= GameManager.Instance.HandleCardSwipe;
            }
        }

        _currentActiveSwipeEffect = newActiveSwipeEffect;
        if (_currentActiveSwipeEffect != null)
        {
            _currentActiveSwipeEffect.cardDestroyed += OnCardDestroyed;

            if (GameManager.Instance != null)
            {
                _currentActiveSwipeEffect.cardSwiped += GameManager.Instance.HandleCardSwipe;
            }
            else
            {
                Debug.LogError("Instantiator: GameManager.Instance is null when trying to subscribe to cardSwiped event.", this);
            }

            _currentActiveCardDisplay = _currentActiveSwipeEffect.GetComponent<CardDisplay>();
            if (_currentActiveCardDisplay == null)
            {
                _currentActiveCardDisplay = _currentActiveSwipeEffect.gameObject.AddComponent<CardDisplay>();
                Debug.LogWarning("Instantiator: Added missing CardDisplay to newly active card.", this);
            }

            if (transform.childCount >= 2)
            {
                GameObject secondCardGameObject = transform.GetChild(0).gameObject;
                SecondCard secondCardComponent = secondCardGameObject.GetComponent<SecondCard>();

                if (secondCardComponent != null)
                {
                    secondCardComponent.SetFirstCardToObserve(newActiveSwipeEffect.gameObject, newActiveSwipeEffect);
                }
                else
                {
                    Debug.LogWarning("Instantiator: No SecondCard component found on the expected second card (at index 0).", this);
                }
            }

            OnActiveCardDisplayReady?.Invoke(_currentActiveCardDisplay);
        }
        else
        {
            Debug.LogError("Instantiator: Attempted to set null SwipeEffect as active.", this);
        }
    }

    void OnCardDestroyed()
    {
        InstantiateNewCard_ForNextSecond();
    }

    public void UpdateActiveCardDisplay(CardData cardData)
    {
        if (_currentActiveCardDisplay != null)
        {
            _currentActiveCardDisplay.SetupCard(cardData);
        }
        else
        {
            // Debug.LogError("Instantiator: No active CardDisplay found to update with new CardData!", this);
        }
    }
}