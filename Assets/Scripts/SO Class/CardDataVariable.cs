// [tooltips] ScriptableVariable để chứa một đối tượng CardData đang hoạt động.
using UnityEngine;
using Obvious.Soap;

[CreateAssetMenu(fileName = "CardDataVariable", menuName = "NguoiKienTao/SOAP/CardData Variable")]
// Đổi tên lớp ở dòng dưới đây
public class CardDataVariable : ScriptableVariable<CardData>
{
    // Lớp này trống vì chỉ dùng để tạo menu item trong Unity.
}