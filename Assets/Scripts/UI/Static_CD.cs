// [tooltips] Cập nhật các thành phần UI tĩnh (tên, lời thoại, hình nền) khi có thẻ mới.
using UnityEngine;
using UnityEngine.UI;
using Obvious.Soap;
using System.Collections.Generic; 
using System.Linq;
using TMPro;

// --- THÊM MỚI: Lớp để map giữa CardType và Sprite ---
[System.Serializable]

public class CardDisplay_Static : MonoBehaviour
{
    [Header("Static UI References")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI leftChoiceText;
    [SerializeField] private TextMeshProUGUI rightChoiceText;
    
    [Header("Date UI")]
    [Tooltip("Ô Text hiển thị ngày tháng năm hiện tại.")]
    [SerializeField] private TextMeshProUGUI currentDateText;
    [Tooltip("Ô Text hiển thị tổng số ngày công tác.")]
    [SerializeField] private TextMeshProUGUI totalDaysWorkedText;

    [Header("Date Data Input")]
    [Tooltip("Biến chứa tổng số ngày công tác.")]
    [SerializeField] private IntVariable totalDaysWorked;
    
    [Header("Background References")]
    [SerializeField] private Image gameBackground;
    [SerializeField] private Sprite defaultBackground;
    [SerializeField] private List<BackgroundMapping> categoryBackgrounds;

    [Header("Game State Input")]
    [SerializeField] private CardDataVariable currentActiveCard;
    
    public void UpdateDisplayOnNewCard()
    {
        CardData cardData = currentActiveCard.Value;
        UpdateTexts(cardData);
        UpdateBackground(cardData); // Gọi hàm cập nhật hình nền
    }
    private void OnEnable()
    {
        if (totalDaysWorked != null)
        {
            // Đăng ký lắng nghe sự thay đổi của biến
            totalDaysWorked.OnValueChanged += UpdateDateDisplay;
            // Cập nhật UI với giá trị ban đầu
            UpdateDateDisplay(totalDaysWorked.Value);
        }
    }
    
    private void OnDisable()
    {
        if (totalDaysWorked != null)
        {
            // Hủy đăng ký để tránh lỗi
            totalDaysWorked.OnValueChanged -= UpdateDateDisplay;
        }
    }
    
    private void UpdateDateDisplay(int totalDays)
    {
        if (currentDateText != null)
        {
            currentDateText.text = $"NGÀY HIỆN TẠI: {ConvertDaysToDateString(totalDays)}";
        }
        if (totalDaysWorkedText != null)
        {
            totalDaysWorkedText.text = $"SỐ NGÀY CÔNG TÁC: {totalDays}";
        }
    }

    // Hàm phụ để chuyển đổi từ tổng số ngày sang định dạng dd/mm/yyyy
    private string ConvertDaysToDateString(int totalDays)
    {
        // Giả sử năm bắt đầu là 2050
        int year = 2050 + (totalDays - 1) / 365;
        int dayOfYear = (totalDays - 1) % 365;
        int month = 1;
        int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        
        while(dayOfYear >= daysInMonth[month-1])
        {
            dayOfYear -= daysInMonth[month-1];
            month++;
        }
        int day = dayOfYear + 1;

        return $"{day:D2}/{month:D2}/{year}";
    }
    private void UpdateTexts(CardData cardData)
    {
        if (cardData != null)
        {
            characterNameText.text = cardData.characterName;
            dialogueText.text = cardData.dialogueText;
            leftChoiceText.text = cardData.leftChoice.choiceText;
            rightChoiceText.text = cardData.rightChoice.choiceText;
        }
        else
        {
            characterNameText.text = "";
            dialogueText.text = "";
            leftChoiceText.text = "";
            rightChoiceText.text = "";
        }
    }
    
    private void UpdateBackground(CardData cardData)
    {
        if (gameBackground == null) return;

        // Nếu không có thẻ, dùng hình mặc định
        if (cardData == null)
        {
            gameBackground.sprite = defaultBackground;
            return;
        }

        Sprite newBackground = null;

        // --- LOGIC ƯU TIÊN MỚI ---
        // Ưu tiên 1: Tìm hình nền dựa trên Ending ID
        if (cardData.cardCategory == StatType.Ending)
        {
            var endingMapping = categoryBackgrounds.FirstOrDefault(m => 
                m.triggerType == BackgroundTriggerType.ByEndingID && 
                m.endingId == cardData.cardID);

            if (endingMapping != null) newBackground = endingMapping.backgroundSprite;
        }

        // Ưu tiên 2: Nếu không phải Ending hoặc không tìm thấy, tìm theo Tên Nhân vật
        if (newBackground == null)
        {
            var charMapping = categoryBackgrounds.FirstOrDefault(m => 
                m.triggerType == BackgroundTriggerType.ByCharacterName && 
                m.characterName.ToString() == cardData.characterName.Replace(" ", "")); // Cần chuẩn hóa tên

            if (charMapping != null) newBackground = charMapping.backgroundSprite;
        }

        // Ưu tiên 3: Nếu vẫn không tìm thấy, tìm theo Loại thẻ (Category)
        if (newBackground == null)
        {
            var categoryMapping = categoryBackgrounds.FirstOrDefault(m => 
                m.triggerType == BackgroundTriggerType.ByCategory && 
                m.cardCategory == cardData.cardCategory);

            if (categoryMapping != null) newBackground = categoryMapping.backgroundSprite;
        }

        // Áp dụng hình nền tìm được, hoặc dùng hình mặc định nếu không có luật nào khớp
        gameBackground.sprite = (newBackground != null) ? newBackground : defaultBackground;
    }
}