// [tooltips] Quản lý việc hiển thị, animation và vòng đời của các GameObject thẻ bài.
using UnityEngine;
using System.Collections.Generic;
using Obvious.Soap;

public class CardViewManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardStackParent;
    
    [Header("Game State Input")]
    [SerializeField] private CardDataVariable currentActiveCard;

    [Header("Config")]
    [SerializeField] private int initialPoolSize = 3;
    [SerializeField] private Vector3 secondCardScale = new Vector3(0.9f, 0.9f, 1f);
    [SerializeField] private Vector3 secondCardPosition = new Vector3(0, -50f, 0);

    private List<GameObject> _cardPool = new List<GameObject>();
    private GameObject _activeCardGO;
    private GameObject _secondCardGO;
    
    private void Start()
    {
        PrepareCardPool();
    }

    private void PrepareCardPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject cardInstance = Instantiate(cardPrefab, cardStackParent);
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
        GameObject newCard = Instantiate(cardPrefab, cardStackParent);
        _cardPool.Add(newCard);
        return newCard;
    }

    
    // Hàm này sẽ được gọi bởi EventListener (không tham số)
    public void OnNewCardReady()
    {
        CardData cardData = currentActiveCard.Value;

        if (_activeCardGO != null && _activeCardGO.activeInHierarchy)
        {
            _secondCardGO = _activeCardGO;
        }

        if (cardData != null)
        {
            _activeCardGO = GetPooledCard();
            _activeCardGO.transform.localPosition = Vector3.zero;
            _activeCardGO.transform.localRotation = Quaternion.identity;
            _activeCardGO.transform.localScale = Vector3.one;

            CardDisplay display = _activeCardGO.GetComponent<CardDisplay>();
            if (display != null)
                display.SetupCard(cardData);

            _activeCardGO.SetActive(true);
            _activeCardGO.transform.SetAsLastSibling();
        }
        else
        {
            if (_activeCardGO != null)
                _activeCardGO.SetActive(false);
            _activeCardGO = null;
        }

        if (_secondCardGO != null)
        {
            int newIndex = _activeCardGO != null ? _activeCardGO.transform.GetSiblingIndex() - 1 : 0;
            _secondCardGO.transform.SetSiblingIndex(Mathf.Max(0, newIndex));
            _secondCardGO.transform.localScale = secondCardScale;
            _secondCardGO.transform.localPosition = secondCardPosition;
        }
    }
}