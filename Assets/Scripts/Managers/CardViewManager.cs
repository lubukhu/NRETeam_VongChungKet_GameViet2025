// [tooltips] Quản lý object pooling, vòng đời và ra lệnh animation cho các thẻ bài động.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

public class CardViewManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject dynamicCardPrefab;
    [SerializeField] private Transform cardStackParent;
    
    [Header("Dependencies")]
    [SerializeField] private UIAnimator uiAnimator;

    [Header("Game State Input")]
    [SerializeField] private CardDataVariable currentActiveCard;

    private List<GameObject> _cardPool = new List<GameObject>();
    private GameObject _activeCardGO;

    private void Start()
    {
        PrepareCardPool();
    }

    private void PrepareCardPool()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cardInstance = Instantiate(dynamicCardPrefab, cardStackParent);
            cardInstance.SetActive(false);
            _cardPool.Add(cardInstance);
        }
    }

    private GameObject GetPooledCard()
    {
        foreach (var card in _cardPool)
        {
            if (!card.activeInHierarchy)
                return card;
        }
        GameObject newCard = Instantiate(dynamicCardPrefab, cardStackParent);
        _cardPool.Add(newCard);
        return newCard;
    }

    public void OnNewCardReady()
    {
        if (currentActiveCard.Value != null)
        {
            Debug.Log("New card ready!");
            _activeCardGO = GetPooledCard();
            
            CardDisplay_Dynamic display = _activeCardGO.GetComponent<CardDisplay_Dynamic>();
            display.Setup(currentActiveCard.Value);
            
            _activeCardGO.SetActive(true);
            _activeCardGO.transform.SetAsLastSibling();
            
            uiAnimator.AnimateFlip(_activeCardGO.transform, null);
        }
    }
    
    public void OnCardSwiped(bool isRightSwipe)
    {
        if (_activeCardGO != null)
        {
            uiAnimator.AnimateCardExit(_activeCardGO.transform, isRightSwipe, () =>
            {
                _activeCardGO.SetActive(false);
                _activeCardGO = null;
            });
        }
    }

    public Transform GetActiveCardTransform()
    {
        return _activeCardGO != null && _activeCardGO.activeInHierarchy ? _activeCardGO.transform : null;
    }
}