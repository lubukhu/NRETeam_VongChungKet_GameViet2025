// [tooltips] Một ScriptableObject định nghĩa một điều kiện để kết thúc game.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;


public enum ConditionCheckType
{
    StatThreshold,      // Kiểm tra ngưỡng của 1 chỉ số
    TimeAndBalance      // Kiểm tra thời gian và sự cân bằng của nhiều chỉ số
}
public enum ComparisonType
{
    LessThanOrEqual,
    GreaterThanOrEqual
}

public enum EndingType { Bad, Good }

[CreateAssetMenu(fileName = "EndingCondition", menuName = "NguoiKienTao/Ending Condition")]
public class EndingCondition : ScriptableObject
{
    [Tooltip("ID của ending sẽ được gửi đi nếu điều kiện đúng.")]
    public string endingId;

    [Tooltip("Loại điều kiện cần kiểm tra.")]
    public ConditionCheckType checkType;
    public EndingType endingType;
    [Header("Settings for 'Stat Threshold'")]
    [Tooltip("Chỉ số cần theo dõi.")]
    public FloatVariable statToWatch;
    [Tooltip("Loại so sánh để kích hoạt.")]
    public ComparisonType comparison;
    [Tooltip("Ngưỡng giá trị để so sánh.")]
    public float threshold;

    [Header("Settings for 'Time And Balance'")]
    [Tooltip("Tổng số ngày công tác yêu cầu.")]
    public int requiredDays = 1095;
    [Tooltip("Danh sách các chỉ số cần phải ở trạng thái cân bằng.")]
    public List<FloatVariable> statsToBalance;
    [Tooltip("Ngưỡng cân bằng tối thiểu.")]
    public float minBalanceThreshold = 10f;
    [Tooltip("Ngưỡng cân bằng tối đa.")]
    public float maxBalanceThreshold = 90f;
}