// [tooltips] "Bộ não" kể chuyện, có khả năng chạy kịch bản tuần tự (tutorial)
// hoặc tạo pool thẻ bài ngẫu nhiên theo trọng số dựa trên trạng thái game.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CategoryWeighting
{
    public StatType category;
    public FloatVariable associatedStat;
    [Range(0f, 10f)]
    public float baseWeight = 1f;
}

public class NarrativeDirector : MonoBehaviour
{
    [Header("Game State Inputs")]
    [SerializeField] private List<CategoryWeighting> categoryWeightings;
    [SerializeField] private IntVariable playthroughCount;

    [Header("Master Deck Input")]
    [Tooltip("Bộ bài tổng hợp đã được xử lý. NarrativeDirector sẽ đọc từ đây.")]
    [SerializeField] private ScriptableListCardData masterDeck;
    
    // XÓA BỎ: Các tham chiếu đến triggeredEndingID và masterDeck_Endings đã được loại bỏ.

    [Header("Runtime Data")]
    [SerializeField] private ScriptableListCardData playedCardsThisRun;
    [SerializeField] private ScriptableListCardData currentCardPool;
    
    [Header("Output Events")]
    [SerializeField] private ScriptableEventNoParam onNewPoolReady;

    [Header("Config")]
    [SerializeField] private int poolSize = 15;

    [Header("Tutorial Settings")]
    [SerializeField] private ScriptableListCardData tutorialDeck;
    [SerializeField] private BoolVariable tutorialCompletedFlag;

    // Hàm này được gọi bởi EventListener lắng nghe onNewPoolRequested
    public void GenerateNextPool()
    {
        currentCardPool.Clear();

        // XÓA BỎ: Khối code kiểm tra ending đã được chuyển sang CardLogicManager.

        // Ưu tiên 1 (trước đây là 2): Kiểm tra Tutorial
        if (playthroughCount.Value == 0 && !tutorialCompletedFlag.Value)
        {
            LoadSequentialDeck(tutorialDeck, tutorialCompletedFlag);
            return;
        }

        // Mặc định: Tạo Pool ngẫu nhiên
        GenerateRandomWeightedPool();
    }

    // XÓA BỎ: Hàm private void LoadEndingCard() không còn cần thiết.
    
    private void LoadSequentialDeck(ScriptableListCardData deck, BoolVariable completionFlag)
    {
        currentCardPool.AddRange(deck);
        if (completionFlag != null)
        {
            completionFlag.Value = true;
        }
        if (onNewPoolReady != null) onNewPoolReady.Raise();
    }

    private void GenerateRandomWeightedPool()
    {
        var categorizedDecks = new Dictionary<StatType, List<CardData>>();
        foreach (var card in masterDeck)
        {
            if (card != null)
            {
                if (!categorizedDecks.ContainsKey(card.cardCategory))
                {
                    categorizedDecks[card.cardCategory] = new List<CardData>();
                }
                categorizedDecks[card.cardCategory].Add(card);
            }
        }

        List<CardData> nextPool = new List<CardData>();
        while (nextPool.Count < poolSize)
        {
            var weights = CalculateWeights();
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
        
        currentCardPool.AddRange(nextPool);
        if (onNewPoolReady != null) onNewPoolReady.Raise();
    }

    private Dictionary<StatType, float> CalculateWeights() {
        var weights = new Dictionary<StatType, float>();
        foreach (var rule in categoryWeightings) {
            float currentWeight = rule.baseWeight;
            if (rule.associatedStat != null) {
                float distanceFromCenter = Mathf.Abs(50f - rule.associatedStat.Value);
                currentWeight += (distanceFromCenter / 50f) * rule.baseWeight;
            }
            weights[rule.category] = currentWeight;
        }
        return weights;
    }

    private StatType GetRandomCategoryByWeight(Dictionary<StatType, float> weights) {
        float totalWeight = weights.Values.Sum();
        if (totalWeight <= 0) return weights.Keys.FirstOrDefault();
        float randomPoint = Random.Range(0, totalWeight);
        foreach (var pair in weights) {
            if (randomPoint < pair.Value) return pair.Key;
            randomPoint -= pair.Value;
        }
        return weights.Keys.LastOrDefault();
    }
}