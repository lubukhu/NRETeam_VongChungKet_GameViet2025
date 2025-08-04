using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(CardAssetCreator))]
public class CardAssetCreatorEditor : Editor
{
    // Nâng cấp 2: Tạo một Dictionary để map từ enum sang tên có dấu
    private static readonly Dictionary<CharacterName, string> characterNameMapping = new Dictionary<CharacterName, string>
    {
        { CharacterName.QuynhHoa, "Cán bộ Quỳnh Hoa" },
        { CharacterName.BacSiDung, "Bác sĩ Dung - Giám đốc bệnh viện" },
        { CharacterName.OngPhat, "Ông Phát - Chủ doanh nghiệp" },
        { CharacterName.AnhKhoi, "Anh Khôi - Thanh niên khởi nghiệp" },
        { CharacterName.ChuTuan, "Chú Tuấn - Thiếu tá Công An" },
        { CharacterName.DaiCaNhanSeo, "Nhân Sẹo - Giang hồ địa phương" },
        { CharacterName.CoNga, "Cô Nga - Hiệu trưởng trường học" },
        { CharacterName.Vy, "Vy - Nhà hoạt động môi trường" },
        { CharacterName.LaoThong, "Lão Thông - Thầy thuốc Nam Y" },
        { CharacterName.TienSiLan, "Tiến sĩ Lan - Nhà khảo cổ" },
        { CharacterName.CuOngAn, "Cụ Ông An - Trưởng làng" },
        { CharacterName.NgheNhanHai, "Anh Hải - Nghệ nhân làng nghề" },
        { CharacterName.VictorNguyen, "Victor Nguyễn - Việt Kiều về nước" },
        { CharacterName.BacNam, "Bác Năm - Đại diện nông dân" },
        { CharacterName.ChuBay, "Chú Bảy - Trưởng ban xây dựng" },
        { CharacterName.KIM, "K.I.M - AI trợ lý" }
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CardAssetCreator creator = (CardAssetCreator)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Create New Card Asset"))
        {
            CreateCardDataAsset(creator);
        }
    }

    private void CreateCardDataAsset(CardAssetCreator creator)
    {
        CardData newCard = ScriptableObject.CreateInstance<CardData>();

        // Nâng cấp 2: Lấy tên có dấu từ Dictionary
        newCard.characterName = characterNameMapping.ContainsKey(creator.selectedCharacter)
            ? characterNameMapping[creator.selectedCharacter]
            : creator.selectedCharacter.ToString();

        // Nâng cấp 3: Gán các giá trị mới
        newCard.cardCategory = creator.cardCategory;
        newCard.frequency = creator.frequency;
        newCard.behaviorType = creator.behaviorType;

        // Nâng cấp 3: Tự động tìm và gán Sprite
        newCard.characterSprite = FindCharacterSprite(creator.selectedCharacter.ToString());

        // Nâng cấp 1: Tạo asset tại thư mục đang chọn
        string assetPath = GetCurrentProjectFolderPath();
        string typeName = creator.cardCategory.ToString();
        string charName = creator.selectedCharacter.ToString();
        string fileName = $"{typeName}_{charName}.asset";
        string fullPath = Path.Combine(assetPath, fileName);
        
        AssetDatabase.CreateAsset(newCard, AssetDatabase.GenerateUniqueAssetPath(fullPath));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newCard;

        Debug.Log($"<color=lime>Successfully created card asset at: {fullPath}</color>");
    }

    private string GetCurrentProjectFolderPath()
    {
        string path = "Assets/ScriptableObjects/Card Assets"; // Mặc định là thư mục Assets

        Object selectedObject = Selection.activeObject;
        if (selectedObject != null)
        {
            string selectedPath = AssetDatabase.GetAssetPath(selectedObject);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (Directory.Exists(selectedPath)) // Nếu đang chọn một thư mục
                {
                    path = selectedPath;
                }
                else // Nếu đang chọn một file
                {
                    path = Path.GetDirectoryName(selectedPath);
                }
            }
        }
        return path;
    }

    private Sprite FindCharacterSprite(string characterEnumName)
    {
        // Để tối ưu, tất cả sprite nhân vật nên được đặt trong một thư mục cụ thể
        string[] searchInFolders = { "Assets/IMG/Characters" }; 
        
        // Tìm kiếm asset có tên trùng với tên enum và có kiểu là Sprite
        string[] guids = AssetDatabase.FindAssets($"{characterEnumName} t:Texture2D", searchInFolders);

        if (guids.Length > 0)
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        }

        Debug.LogWarning($"Could not find sprite for: {characterEnumName}. Please ensure the sprite exists in '{searchInFolders[0]}' and is named correctly.");
        return null;
    }
}