// [tooltips] Chịu trách nhiệm hiển thị dữ liệu từ một ScriptableObject CardData lên các thành phần UI của thẻ bài.
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI leftChoiceText;
    public TextMeshProUGUI rightChoiceText;
    
    public void SetupCard(CardData cardData)
    {
        if (cardData == null)
        {
            // Trường hợp này có thể xảy ra khi hết thẻ, nên chỉ cần ẩn đi là được
            gameObject.SetActive(false);
            return;
        }
        
        // Cập nhật hình ảnh và tên nhân vật
        if (characterImage != null)
        {
            characterImage.sprite = cardData.characterSprite;
            characterImage.enabled = (cardData.characterSprite != null);
        }
        if (characterNameText != null)
        {
            characterNameText.text = cardData.characterName;
        }

        // Cập nhật hội thoại
        if (dialogueText != null)
        {
            dialogueText.text = cardData.dialogueText;
        }

        // Cập nhật nội dung lựa chọn từ cấu trúc mới
        if (leftChoiceText != null)
        {
            leftChoiceText.text = cardData.leftChoice.choiceText;
        }
        if (rightChoiceText != null)
        {
            // Ẩn text lựa chọn phải nếu đây là thẻ Narrative
            bool isDecisionCard = (cardData.behaviorType == CardBehaviorType.Decision);
            rightChoiceText.gameObject.SetActive(isDecisionCard);
            if (isDecisionCard)
            {
                rightChoiceText.text = cardData.rightChoice.choiceText;
            }
        }
    }
}