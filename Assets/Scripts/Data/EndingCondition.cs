// [tooltips] Một ScriptableObject định nghĩa một điều kiện để kết thúc game.
using UnityEngine;
using Obvious.Soap;

public enum ComparisonType
{
    LessThanOrEqual,
    GreaterThanOrEqual
}

[CreateAssetMenu(fileName = "EndingCondition", menuName = "NguoiKienTao/Ending Condition")]
public class EndingCondition : ScriptableObject
{
    [Tooltip("Chỉ số cần theo dõi.")]
    public FloatVariable statToWatch;

    [Tooltip("Loại so sánh để kích hoạt.")]
    public ComparisonType comparison;

    [Tooltip("Ngưỡng giá trị để so sánh.")]
    public float threshold;

    [Tooltip("ID của ending sẽ được gửi đi nếu điều kiện đúng.")]
    public string endingId;
}