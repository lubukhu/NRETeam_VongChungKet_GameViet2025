// [tooltips] Quản lý bộ bài hiện tại (currentCardPool) và các thao tác rút/hủy thẻ.
using UnityEngine;
using Obvious.Soap;

public class DeckManager : MonoBehaviour
{
    [Header("Decks")]
    [SerializeField] private ScriptableListCardData currentCardPool;
    [SerializeField] private ScriptableListCardData playedCardsThisRun;

    [Header("Narrative Inputs")]
    [Tooltip("Hàng đợi ưu tiên. Thẻ bài sẽ luôn được rút từ đây trước.")]
    [SerializeField] private ScriptableListCardData forcedCardsQueue;
    
    [Header("Output Events")]
    [SerializeField] private ScriptableEventNoParam onNewCardReady;
    [SerializeField] private ScriptableEventNoParam onNewPoolRequested;
    [SerializeField] private ScriptableEventNoParam onPrefabInstantiate;

    [Header("Game State")]
    [SerializeField] private CardDataVariable currentActiveCard;
    [SerializeField] private GameObjectVariable currentDisplayedCardGameObject;

    [Header("Card Prefab")]
    [SerializeField] private GameObject dynamicCardPrefab;
    [SerializeField] private Transform cardSpawnParent;

    private GameObject _currentActiveCardGO;

    // Hàm này vẫn do EventListener gọi để bắt đầu game
    public void InitializeNewGame()
    {
        DestroyCurrentCardObject();
        playedCardsThisRun.Clear();
        currentCardPool.Clear();
        currentActiveCard.Value = null;
        if (currentDisplayedCardGameObject != null)
        {
            currentDisplayedCardGameObject.Value = null;
        }
        onNewPoolRequested.Raise();
    }
    
    // Hàm rút thẻ giờ đây cực kỳ đơn giản
    public void DrawNextCard()
    {
        DestroyCurrentCardObject();

        if (IsCardPoolEmpty() && (forcedCardsQueue == null || forcedCardsQueue.Count == 0))
        {
            onNewPoolRequested.Raise();
            return;
        }

        CardData nextCardData = SelectAndProcessNextCard();
        SpawnNewCard(nextCardData);
    }
    
    // HÀM RequestMoreCardsIfNeeded() ĐÃ BỊ XÓA BỎ HOÀN TOÀN

    private void DestroyCurrentCardObject()
    {
        if (_currentActiveCardGO != null)
        {
            Destroy(_currentActiveCardGO);
            _currentActiveCardGO = null;
        }
    }
    
    private CardData SelectAndProcessNextCard()
    {
        CardData nextCardData;
        
        if (forcedCardsQueue != null && forcedCardsQueue.Count > 0)
        {
            nextCardData = forcedCardsQueue[0];
            forcedCardsQueue.RemoveAt(0);
        }
        else
        {
            nextCardData = currentCardPool[0];
            currentCardPool.RemoveAt(0);
        }

        if (nextCardData.frequency == CardFrequency.OncePerPlaythrough && !playedCardsThisRun.Contains(nextCardData))
        {
            playedCardsThisRun.Add(nextCardData);
        }
        return nextCardData;
    }

    private void SpawnNewCard(CardData cardData)
    {
        _currentActiveCardGO = Instantiate(dynamicCardPrefab, cardSpawnParent);
        _currentActiveCardGO.transform.localPosition = Vector3.zero;
        _currentActiveCardGO.transform.SetAsLastSibling();
        
        if (currentDisplayedCardGameObject != null)
        {
            currentDisplayedCardGameObject.Value = _currentActiveCardGO;
        }
        currentActiveCard.Value = cardData;

        if (onPrefabInstantiate != null)
        {
            onPrefabInstantiate.Raise();
        }
        onNewCardReady.Raise();
    }
    
    private bool IsCardPoolEmpty()
    {
        return currentCardPool.Count == 0;
    }
    
    public void ForceSpawnSpecificCard(CardData cardToSpawn)
    {
        DestroyCurrentCardObject();
        SpawnNewCard(cardToSpawn);
    }
}