// [tooltips] Quản lý vòng đời của các bộ bài, xử lý việc rút thẻ và yêu cầu tạo pool mới khi cần.
using UnityEngine;
using Obvious.Soap;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    [Header("Decks")]
    [SerializeField] private ScriptableListCardData currentCardPool;
    [SerializeField] private ScriptableListCardData playedCardsThisRun;

    [Header("Output Events")]
    [SerializeField] private ScriptableEventNoParam onNewCardReady; 
    [SerializeField] private ScriptableEventNoParam onNewPoolRequested;

    [Header("Game State")]
    [SerializeField] private CardDataVariable currentActiveCard;

    // Hàm này sẽ được gọi bởi EventListener sau khi UnlockManager đã xử lý xong.
    public void InitializeNewGame()
    {
        playedCardsThisRun.Clear();
        currentCardPool.Clear();

        // Bắt đầu game bằng cách yêu cầu ngay một pool ngẫu nhiên
        onNewPoolRequested.Raise();
    }

    public void DrawNextCard()
    {
        if (currentCardPool.Count == 0)
        {
            // Nếu pool trống, yêu cầu pool mới (trường hợp đặc biệt khi khởi đầu)
            onNewPoolRequested.Raise();
            return;
        }

        CardData nextCard = currentCardPool[0]; // gán card tiếp theo = card ở vị trí đầu tiên của cardpool hiện tại
        currentCardPool.RemoveAt(0);

        if (nextCard.frequency == CardFrequency.OncePerPlaythrough)
        {
            if (!playedCardsThisRun.Contains(nextCard))
                playedCardsThisRun.Add(nextCard);
        }

        currentActiveCard.Value = nextCard;
        onNewCardReady.Raise();

        if (currentCardPool.Count <= 1)
        {
            onNewPoolRequested.Raise();
        }
    }
}