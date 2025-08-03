// [tooltips] Quản lý logic thời gian trong game, chỉ tính toán và cập nhật dữ liệu.
using UnityEngine;
using Obvious.Soap;

public class DateManager : MonoBehaviour
{
    [Header("Game State Data")]
    [Tooltip("Biến lưu tổng số ngày công tác đã trôi qua.")]
    [SerializeField] private IntVariable totalDaysWorked;
    [Tooltip("Thẻ bài đang hoạt động để kiểm tra loại.")]
    [SerializeField] private CardDataVariable currentActiveCard;

    [Header("Config")]
    [Tooltip("Khoảng ngẫu nhiên cho số ngày trôi qua mỗi khi vuốt thẻ (X: min, Y: max).")]
    [SerializeField] private Vector2Int daysPerCardRange = new Vector2Int(10, 20);

    // Hàm này sẽ được gọi bởi EventListener lắng nghe sự kiện OnCardSwiped
    public void AdvanceTime()
    {
        if (totalDaysWorked == null || currentActiveCard == null || currentActiveCard.Value == null) return;

        // --- KIỂM TRA LOẠI THẺ BÀI ---
        // Chỉ tăng thời gian nếu thẻ được vuốt thuộc 4 loại chỉ số chính.
        StatType cardCategory = currentActiveCard.Value.cardCategory;
        bool isStatCard = cardCategory == StatType.Finance ||
                          cardCategory == StatType.Trust ||
                          cardCategory == StatType.Environment ||
                          cardCategory == StatType.Culture;

        if (!isStatCard)
        {
            // Nếu không phải thẻ chỉ số (ví dụ: Tutorial, Ending), không làm gì cả.
            return;
        }
        
        // Tính toán và cập nhật tổng số ngày công tác
        int daysToAdd = Random.Range(daysPerCardRange.x, daysPerCardRange.y + 1);
        totalDaysWorked.Value += daysToAdd;
    }
}