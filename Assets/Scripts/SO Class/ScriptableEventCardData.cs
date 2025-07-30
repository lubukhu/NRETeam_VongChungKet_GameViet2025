// [tooltips] ScriptableEvent để truyền đi một đối tượng CardData.
using UnityEngine;
using Obvious.Soap;

[CreateAssetMenu(fileName = "ScriptableEventCardData", menuName = "NguoiKienTao/SOAP/Event (CardData)")]
public class ScriptableEventCardData : ScriptableEvent<CardData>
{
}