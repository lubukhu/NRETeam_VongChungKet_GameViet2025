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

    private CardData _currentCardData;

    public void SetupCard(CardData cardData)
    {
        if (cardData == null)
        {
            Debug.LogError("CardDisplay: Received null CardData!", this);
            return;
        }

        _currentCardData = cardData;

        if (characterImage != null && cardData.characterSprite != null)
        {
            characterImage.sprite = cardData.characterSprite;
        }
        else if (characterImage != null)
        {
            characterImage.sprite = null;
        }

        if (characterNameText != null)
        {
            characterNameText.text = cardData.characterName;
        }
        if (dialogueText != null)
        {
            dialogueText.text = cardData.dialogueText;
        }
        if (leftChoiceText != null)
        {
            leftChoiceText.text = cardData.leftChoiceText;
        }
        if (rightChoiceText != null)
        {
            rightChoiceText.text = cardData.rightChoiceText;
        }
    }

    public CardData GetCurrentCardData()
    {
        return _currentCardData;
    }
}