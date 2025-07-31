// [tooltips] Cập nhật các thành phần UI tĩnh (tên, lời thoại) khi có thẻ mới.
using UnityEngine;
using Obvious.Soap;
using TMPro;

public class CardDisplay_Static : MonoBehaviour
{
    [Header("Static UI References")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI leftChoiceText;
    [SerializeField] private TextMeshProUGUI rightChoiceText;

    [Header("Game State Input")]
    [SerializeField] private CardDataVariable currentActiveCard; // Lắng nghe sự thay đổi của biến này

    public void UpdateStaticTexts()
    {
        CardData cardData = currentActiveCard.Value;
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
            leftChoiceText.text = ""; // Đảm bảo clear text
            rightChoiceText.text = ""; // Đảm bảo clear text
        }
    }
}