using UnityEngine;
using Obvious.Soap;

// Enum chứa tất cả các nhân vật
public enum CharacterName
{
    QuynhHoa, BacSiDung, OngPhat, AnhKhoi, ChuTuan, DaiCaNhanSeo,
    CoNga, Vy, LaoThong, TienSiLan, CuOngAn, NgheNhanHai, VictorNguyen,
    BacNam, ChuBay, KIM
}

public class CardAssetCreator : MonoBehaviour
{
    [Header("Card Properties")]
    public StatType cardCategory;
    public CharacterName selectedCharacter;

    // --- THÊM MỚI ---

    public CardFrequency frequency = CardFrequency.Standard;
    
    public CardBehaviorType behaviorType = CardBehaviorType.Decision;
    // --- KẾT THÚC THÊM MỚI ---
}