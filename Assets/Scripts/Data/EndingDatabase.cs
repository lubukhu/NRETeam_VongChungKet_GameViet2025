// [tooltips] Một ScriptableObject chứa danh sách tất cả các thông tin về Ending có thể có.
using UnityEngine;
using System.Collections.Generic;

// Lớp cấu trúc dữ liệu cho một Ending duy nhất
[System.Serializable]
public class EndingInfo
{
    [Tooltip("ID phải khớp với ID trong EndingCondition.")]
    public string endingId;
    [Tooltip("Hình ảnh đại diện cho ending.")]
    public Sprite endingSprite;

    public string endingDescription;
}

[CreateAssetMenu(fileName = "EndingDatabase", menuName = "NguoiKienTao/Database/Ending Database")]
public class EndingDatabase : ScriptableObject
{
    [Tooltip("Danh sách tất cả các ending có trong game.")]
    public List<EndingInfo> allEndings;
}