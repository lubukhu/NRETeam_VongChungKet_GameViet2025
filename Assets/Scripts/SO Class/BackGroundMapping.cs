using UnityEngine;
using Obvious.Soap; // Cần thiết để dùng StatType

// Enum để định nghĩa loại điều kiện
public enum BackgroundTriggerType
{
    ByCategory,       
    ByCharacterName,  
    ByEndingID        
}

[System.Serializable]
public class BackgroundMapping
{
    [Tooltip("Loại điều kiện để kích hoạt hình nền này.")]
    public BackgroundTriggerType triggerType;
    public Sprite backgroundSprite;
    

    [Tooltip("Chọn Category nếu Trigger Type là 'ByCategory'.")]
    public StatType cardCategory;

    [Tooltip("Chọn Tên Nhân vật nếu Trigger Type là 'ByCharacterName'.")]
    public CharacterName characterName; // Tận dụng enum CharacterName đã có

    [Tooltip("Nhập ID của Ending nếu Trigger Type là 'ByEndingID'.")]
    public string endingId;
}