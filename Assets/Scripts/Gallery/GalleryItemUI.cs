using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText; // Thêm tham chiếu đến Text

    public void Setup(Sprite sprite, string name)
    {
        if (itemImage != null)
        {
            itemImage.sprite = sprite;
            itemImage.color = Color.white;
        }
        if (itemNameText != null)
        {
            itemNameText.text = name;
        }
    }

    public void SetLockedState(Sprite lockedSprite, string lockedName = "???")
    {
        if (itemImage != null)
        {
            itemImage.sprite = lockedSprite;
            //itemImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        }
        if (itemNameText != null)
        {
            itemNameText.text = lockedName;
        }
    }
}