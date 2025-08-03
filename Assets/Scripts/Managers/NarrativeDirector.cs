// [tooltips] "Bộ não" kể chuyện, có khả năng chạy kịch bản tuần tự (tutorial)
// hoặc tạo pool thẻ bài ngẫu nhiên theo trọng số dựa trên trạng thái game.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;
using System.Linq;

// Lớp cấu trúc dữ liệu mới để Game Designer định nghĩa luật trọng số trong Inspector
[System.Serializable]
public class CategoryWeighting
{
    [Tooltip("Loại thẻ bài mà quy tắc này áp dụng.")]
    public StatType category;

    [Tooltip("Chỉ số liên quan (nếu có). Để trống nếu đây là loại thẻ không gắn với chỉ số.")]
    public FloatVariable associatedStat;

    [Tooltip("Trọng số cơ bản, không đổi.")]
    [Range(0f, 10f)]
    public float baseWeight = 1f;
}

public class NarrativeDirector : MonoBehaviour
{
    [Header("Game State Inputs")]
    [Tooltip("Danh sách các quy tắc tính trọng số cho từng loại thẻ.")]
    [SerializeField] private List<CategoryWeighting> categoryWeightings;
    [SerializeField] private IntVariable playthroughCount;

    [Header("Master Deck Input")]
    [Tooltip("Bộ bài tổng hợp đã được xử lý. NarrativeDirector sẽ đọc từ đây.")]
    [SerializeField] private ScriptableListCardData masterDeck;
    
    [Header("Runtime Data")]
    [SerializeField] private ScriptableListCardData playedCardsThisRun;
    [SerializeField] private ScriptableListCardData currentCardPool;
    
    [Header("Deck State Output")]
    [Tooltip("Tín hiệu để báo cho các hệ thống khác biết loại bộ bài đang được chơi.")]
    [SerializeField] private BoolVariable isSequentialDeckActive;
    [SerializeField] private ScriptableEventNoParam onNewPoolReady;

    [Header("Config")] [SerializeField] private int poolSize = 10;

    [Header("Tutorial Settings")]
    [Tooltip("Bộ bài sẽ được nạp trong lần chơi đầu tiên.")]
    [SerializeField] private ScriptableListCardData tutorialDeck;
    [Tooltip("Cờ để đánh dấu tutorial đã hoàn thành.")]
    [SerializeField] private BoolVariable tutorialCompletedFlag;

    // Hàm này được gọi bởi EventListener lắng nghe onNewPoolRequested
    public void GenerateNextPool()
    {
        currentCardPool.Clear();

        // CỔNG KIỂM TRA ƯU TIÊN: Luôn kiểm tra điều kiện tutorial trước tiên.
        if (playthroughCount.Value == 0 && !tutorialCompletedFlag.Value)
        {
            LoadSequentialDeck(tutorialDeck, tutorialCompletedFlag);
            return; // Dừng lại sau khi nạp xong tutorial deck
        }

        // Nếu không phải tutorial, chạy logic tạo pool ngẫu nhiên theo trọng số
        GenerateRandomWeightedPool();
    }

    private void LoadSequentialDeck(ScriptableListCardData deck, BoolVariable completionFlag)
    {
        if (deck == null) {
            return;
        }
        currentCardPool.AddRange(deck);
        if (completionFlag != null)
        {
            completionFlag.Value = true;
        }
        
        if (isSequentialDeckActive != null)
        {
            isSequentialDeckActive.Value = true;
        }
        
        if (onNewPoolReady != null) onNewPoolReady.Raise();
    }

    private void GenerateRandomWeightedPool()
    {
        var categorizedDecks = new Dictionary<StatType, List<CardData>>();
        foreach (var card in masterDeck)
        {
            if (card == null) continue;
            if (!categorizedDecks.ContainsKey(card.cardCategory))
            {
                categorizedDecks[card.cardCategory] = new List<CardData>();
            }
            categorizedDecks[card.cardCategory].Add(card);
        }

        var weights = CalculateWeights();
        List<CardData> nextPool = new List<CardData>();

        while (nextPool.Count < poolSize)
        {
            var availableCategories = new Dictionary<StatType, float>();
            foreach (var pair in weights)
            {
                StatType category = pair.Key;
                if (categorizedDecks.ContainsKey(category) && categorizedDecks[category].Any(c => !playedCardsThisRun.Contains(c) && !nextPool.Contains(c)))
                {
                    availableCategories[category] = pair.Value;
                }
            }

            if (availableCategories.Count == 0) break;

            StatType chosenCategory = GetRandomCategoryByWeight(availableCategories);
            var potentialCards = categorizedDecks[chosenCategory]
                .Where(c => !playedCardsThisRun.Contains(c) && !nextPool.Contains(c))
                .ToList();

            if (potentialCards.Count > 0)
            {
                nextPool.Add(potentialCards[Random.Range(0, potentialCards.Count)]);
            }
        }
        
        if (isSequentialDeckActive != null)
        {
            isSequentialDeckActive.Value = false;
        }
        
        currentCardPool.AddRange(nextPool);
        
        if (onNewPoolReady != null) onNewPoolReady.Raise();
    }

    private Dictionary<StatType, float> CalculateWeights()
    {
        var weights = new Dictionary<StatType, float>();
        foreach (var rule in categoryWeightings)
        {
            float currentWeight = rule.baseWeight;
            if (rule.associatedStat != null)
            {
                float distanceFromCenter = Mathf.Abs(50f - rule.associatedStat.Value);
                currentWeight += (distanceFromCenter / 50f) * rule.baseWeight;
            }
            weights[rule.category] = currentWeight;
        }
        return weights;
    }

    private StatType GetRandomCategoryByWeight(Dictionary<StatType, float> weights)
    {
        float totalWeight = weights.Values.Sum();
        if (totalWeight <= 0) return weights.Keys.FirstOrDefault();
        
        float randomPoint = Random.Range(0, totalWeight);
        foreach (var pair in weights)
        {
            if (randomPoint < pair.Value) return pair.Key;
            randomPoint -= pair.Value;
        }
        return weights.Keys.LastOrDefault();
    }
}