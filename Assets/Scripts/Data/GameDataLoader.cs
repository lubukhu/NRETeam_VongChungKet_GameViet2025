// [tooltips] Script duy nhất chịu trách nhiệm nạp dữ liệu cấu hình ban đầu từ Editor vào các ScriptableList lúc runtime.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

// --- Các lớp cấu trúc dữ liệu để hiển thị gọn gàng trong Inspector ---
// Mỗi lớp định nghĩa một cặp "Nguồn" (danh sách trong Editor) và "Đích" (asset ScriptableList).

[System.Serializable]
public class DeckLoadingSet
{
    [Tooltip("Mô tả để Game Designer dễ nhận biết (ví dụ: 'Bộ bài cơ bản của Ông Phát').")]
    public string description;
    [Tooltip("Danh sách các CardData gốc.")]
    public List<CardData> sourceCards;
    [Tooltip("Asset ScriptableList sẽ nhận các thẻ bài này lúc runtime.")]
    public ScriptableListCardData targetDeck;
}

[System.Serializable]
public class RuleLoadingSet
{
    public string description;
    public List<UnlockRule> sourceRules;
    public ScriptableListUnlockRule targetRuleList;
}

/*
// --- CÁC CẤU TRÚC DỮ LIỆU CHỜ ĐỂ MỞ RỘNG TRONG TƯƠNG LAI ---

[System.Serializable]
public class EndingLoadingSet
{
    public string description;
    // Bạn sẽ cần tạo lớp "EndingData" và "ScriptableListEndingData"
    // public List<EndingData> sourceEndings;
    // public ScriptableListEndingData targetEndingList;
}

[System.Serializable]
public class ItemLoadingSet
{
    public string description;
    // Bạn sẽ cần tạo lớp "ItemData" và "ScriptableListItemData"
    // public List<ItemData> sourceItems;
    // public ScriptableListItemData targetItemList;
}
*/


// --- LỚP LOADER CHÍNH ---

public class GameDataLoader : MonoBehaviour
{
    [Header("Deck Loading")]
    [SerializeField] private List<DeckLoadingSet> decksToLoad;
    
    [Header("Game Rule Loading")]
    [SerializeField] private List<RuleLoadingSet> rulesToLoad;
    
    /*
    [Header("Ending Loading")]
    [SerializeField] private List<EndingLoadingSet> endingsToLoad;
    
    [Header("Item Loading")]
    [SerializeField] private List<ItemLoadingSet> itemsToLoad;
    */

    [Header("Output Events")]
    [Tooltip("Sự kiện được phát ra sau khi TẤT CẢ dữ liệu đã được nạp xong.")]
    [SerializeField] private ScriptableEventNoParam onMasterDataLoaded; // Đổi tên để rõ nghĩa hơn

    // Sử dụng Start() thay vì Awake() để đảm bảo tất cả các listener đã sẵn sàng
    private void Start()
    {
        LoadAllData();
    }

    private void LoadAllData()
    {
        // Nạp tất cả các bộ bài
        foreach (var set in decksToLoad)
        {
            if (set.targetDeck != null && set.sourceCards != null)
            {
                set.targetDeck.Clear();
                set.targetDeck.AddRange(set.sourceCards);
            }
        }

        // Nạp tất cả các luật chơi
        foreach (var set in rulesToLoad)
        {
            if (set.targetRuleList != null && set.sourceRules != null)
            {
                set.targetRuleList.Clear();
                set.targetRuleList.AddRange(set.sourceRules);
            }
        }
        
        // Khi cần mở rộng, chỉ cần thêm vòng lặp foreach tương ứng cho Endings, Items...
        /*
        foreach (var set in endingsToLoad)
        {
            // ...
        }
        */

        Debug.Log("GameDataLoader: All master data has been loaded into ScriptableLists.");

        // Phát tín hiệu để bắt đầu chuỗi khởi tạo game (UnlockManager sẽ lắng nghe)
        if (onMasterDataLoaded != null)
        {
            onMasterDataLoaded.Raise();
        }
    }
}