// [tooltips] Cập nhật hình ảnh cho thẻ bài động và phát tín hiệu animation lật thẻ.
using UnityEngine;
using UnityEngine.UI;
using Obvious.Soap; 

public class CardDisplay_Dynamic : MonoBehaviour
{
    [SerializeField] private Image characterImage;

    [Header("Game State Input")]
    [SerializeField] private CardDataVariable currentActiveCard;
    
    [Header("Output Events")]
    [SerializeField] private ScriptableEventNoParam FlipPrefabCard; // Vẫn là NoParam!

    // Phương thức này sẽ được gọi bởi EventListener khi 'onPrefabInstantiate' từ DeckManager được Raise.
    public void OnBeingInstantiate() 
    {
        if (FlipPrefabCard != null)
        {
            FlipPrefabCard.Raise(); // Vẫn Raise mà không truyền tham số
        }
    }

    public void OnNewCardDataReady() 
    {
        if (currentActiveCard.Value != null)
        {
            if (characterImage != null)
            {
                characterImage.sprite = currentActiveCard.Value.characterSprite;
            }
        }
    }
}