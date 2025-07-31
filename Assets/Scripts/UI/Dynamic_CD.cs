// File: CardDisplay_Dynamic.cs
// [tooltips] Cập nhật hình ảnh cho thẻ bài động.
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay_Dynamic : MonoBehaviour
{
    [SerializeField] private Image characterImage;

    public void Setup(CardData cardData)
    {
        if (characterImage != null)
        {
            characterImage.sprite = cardData.characterSprite;
        }
    }
}