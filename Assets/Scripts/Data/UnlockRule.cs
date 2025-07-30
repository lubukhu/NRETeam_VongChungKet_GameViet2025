// [tooltips] Một ScriptableObject định nghĩa các điều kiện và hành động để mở khóa nội dung mới.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnlockRule", menuName = "NguoiKienTao/Unlock Rule")]
public class UnlockRule : ScriptableObject
{
    [Header("Conditions")]
    [Tooltip("Số lượt chơi tối thiểu cần đạt được. Đặt là 0 nếu không cần.")]
    public int requiredPlaythroughs = 0;

    [Tooltip("Danh sách các 'cờ' trạng thái phải có giá trị là TRUE.")]
    public List<BoolVariable> requiredFlags;

    [Header("Actions")]
    [Tooltip("Danh sách các thẻ bài sẽ được thêm vào bộ bài mục tiêu.")]
    public List<CardData> cardsToAdd;

    [Tooltip("Bộ bài gốc sẽ nhận các thẻ bài mới khi quy tắc này được thỏa mãn.")]
    public ScriptableListCardData targetDeck;
}