// [tooltips] Quản lý vòng đời của các bộ bài, xử lý việc rút thẻ, khởi tạo/hủy thẻ bài vật lý, và yêu cầu tạo pool mới khi cần.
using UnityEngine;
using Obvious.Soap;
using System.Linq;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    [Header("Decks")]
    [SerializeField] private ScriptableListCardData currentCardPool;
    [SerializeField] private ScriptableListCardData playedCardsThisRun;

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
    
    [Header("Game State Inputs")]
    [Tooltip("Tín hiệu để biết bộ bài hiện tại có phải là loại tuần tự hay không.")]
    [SerializeField] private BoolVariable isSequentialDeckActive;

    private GameObject _currentActiveCardGO;

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
    
    public void DrawNextCard()
    {
        DestroyCurrentCardObject();

        // Guard Clause: Dừng lại ngay nếu hết bài để rút
        if (IsCardPoolEmpty())
        {
            onNewPoolRequested.Raise();
            return;
        }

        // Lấy dữ liệu thẻ bài tiếp theo từ bộ bài
        CardData nextCardData = SelectAndProcessNextCard();

        // Dựa vào dữ liệu, tạo thẻ bài vật lý trên màn hình
        SpawnNewCard(nextCardData);

        // Kiểm tra và yêu cầu thêm bài nếu bộ bài sắp hết
        RequestMoreCardsIfNeeded();
    }
    
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
        CardData nextCardData = currentCardPool[0];
        currentCardPool.RemoveAt(0);

        // Xử lý các thẻ chỉ xuất hiện một lần
        if (nextCardData.frequency == CardFrequency.OncePerPlaythrough && !playedCardsThisRun.Contains(nextCardData))
        {
            playedCardsThisRun.Add(nextCardData);
        }

        return nextCardData;
    }


    private void SpawnNewCard(CardData cardData)
    {
        // 1. Khởi tạo GameObject
        _currentActiveCardGO = Instantiate(dynamicCardPrefab, cardSpawnParent);
        _currentActiveCardGO.transform.localPosition = Vector3.zero;
        _currentActiveCardGO.transform.SetAsLastSibling();

        // 2. Cập nhật các ScriptableObject trạng thái
        if (currentDisplayedCardGameObject != null)
        {
            currentDisplayedCardGameObject.Value = _currentActiveCardGO;
        }
        currentActiveCard.Value = cardData;

        // 3. Phát sự kiện báo hiệu
        if (onPrefabInstantiate != null)
        {
            onPrefabInstantiate.Raise();
        }
        onNewCardReady.Raise();
    }


    private void RequestMoreCardsIfNeeded()
    {
        if (isSequentialDeckActive != null && isSequentialDeckActive.Value)
        {
            return; // Nếu là bộ bài tuần tự, không làm gì cả.
        }
        
        if (currentCardPool.Count <= 1)
        {
            onNewPoolRequested.Raise();
        }
    }
    
    private bool IsCardPoolEmpty()
    {
        return currentCardPool.Count == 0;
    }
}