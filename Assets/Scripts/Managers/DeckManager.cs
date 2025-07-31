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

    /// <summary>
    /// Hàm chính điều phối việc rút thẻ bài tiếp theo.
    /// </summary>
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

    /// <summary>
    /// Hủy GameObject của thẻ bài hiện tại nếu có.
    /// </summary>
    private void DestroyCurrentCardObject()
    {
        if (_currentActiveCardGO != null)
        {
            Destroy(_currentActiveCardGO);
            _currentActiveCardGO = null;
        }
    }

    /// <summary>
    /// Lấy thẻ bài đầu tiên từ pool, xử lý logic đặc biệt và trả về.
    /// </summary>
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

    /// <summary>
    /// Tạo GameObject thẻ bài mới, thiết lập và cập nhật trạng thái game.
    /// </summary>
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

    /// <summary>
    /// Kiểm tra xem bộ bài có đang cạn kiệt hay không để yêu cầu pool mới.
    /// </summary>
    private void RequestMoreCardsIfNeeded()
    {
        if (currentCardPool.Count <= 1)
        {
            onNewPoolRequested.Raise();
        }
    }

    /// <summary>
    /// Kiểm tra xem bộ bài có còn thẻ hay không.
    /// </summary>
    /// <returns>True nếu bộ bài rỗng.</returns>
    private bool IsCardPoolEmpty()
    {
        return currentCardPool.Count == 0;
    }
}