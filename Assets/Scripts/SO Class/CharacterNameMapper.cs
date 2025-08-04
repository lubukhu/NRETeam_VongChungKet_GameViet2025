using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CharacterNameMapper", menuName = "NguoiKienTao/Character Name Mapper")]
public class CharacterNameMapper : ScriptableObject
{
    // "Nguồn chân lý" - Dictionary từ Enum sang tên hiển thị đầy đủ
    private static readonly Dictionary<CharacterName, string> _characterMap = new Dictionary<CharacterName, string>
    {
        { CharacterName.QuynhHoa, "Thư ký Quỳnh Hoa" },
        { CharacterName.BacSiDung, "Bác sĩ Dung - Giám đốc bệnh viện" },
        { CharacterName.OngPhat, "Ông Phát - Chủ doanh nghiệp" },
        { CharacterName.AnhKhoi, "Anh Khôi - Thanh niên khởi nghiệp" },
        { CharacterName.ChuTuan, "Chú Tuấn - Thiếu tá Công An" },
        { CharacterName.DaiCaNhanSeo, "Nhân Sẹo - Giang hồ địa phương" },
        { CharacterName.CoNga, "Cô Nga - Hiệu trưởng trường học" },
        { CharacterName.Vy, "Vy - Nhà hoạt động môi trường" },
        { CharacterName.LaoThong, "Lão Thông - Thầy thuốc Nam Y" },
        { CharacterName.TienSiLan, "Tiến sĩ Lan - Nhà khảo cổ" },
        { CharacterName.CuOngAn, "Cụ ông An - Trưởng làng" },
        { CharacterName.NgheNhanHai, "Anh Hải - Nghệ nhân làng nghề" },
        { CharacterName.VictorNguyen, "Victor Nguyễn - Việt Kiều về nước" },
        { CharacterName.BacNam, "Bác Năm - Đại diện nông dân" },
        { CharacterName.ChuBay, "Chú Bảy - Trưởng ban xây dựng" },
        { CharacterName.KIM, "K.I.M - AI trợ lý" }
    };

    // Dictionary tra cứu ngược, được tự động tạo ra để tăng tốc độ
    private static readonly Dictionary<string, CharacterName> _reverseMap = 
        _characterMap.ToDictionary(pair => pair.Value, pair => pair.Key);
    
    public string GetDisplayName(CharacterName characterEnum)
    {
        if (_characterMap.TryGetValue(characterEnum, out string displayName))
        {
            return displayName;
        }
        return characterEnum.ToString(); // Trả về tên enum nếu không tìm thấy
    }
    
    public bool TryGetEnum(string displayName, out CharacterName characterEnum)
    {
        if (displayName == null)
        {
            characterEnum = default;
            return false;
        }
        return _reverseMap.TryGetValue(displayName, out characterEnum);
    }
}