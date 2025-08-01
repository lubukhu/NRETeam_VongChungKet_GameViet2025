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

        if (cardData == null)
        {
            gameBackground.sprite = defaultBackground;
            return;
        }
        
        var mapping = categoryBackgrounds.FirstOrDefault(m => m.cardCategory == cardData.cardCategory);

        if (mapping != null && mapping.backgroundSprite != null)
        {
            gameBackground.sprite = mapping.backgroundSprite;
        }
        else
        {
            gameBackground.sprite = defaultBackground;
        }
    }
}