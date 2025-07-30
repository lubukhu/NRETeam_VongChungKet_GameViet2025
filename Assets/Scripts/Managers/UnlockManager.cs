// [tooltips] Kiểm tra các quy tắc mở khóa và thêm nội dung mới vào các bộ bài gốc.
using UnityEngine;
using Obvious.Soap;
using System.Linq;

public class UnlockManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private ScriptableListUnlockRule allUnlockRules;
    [SerializeField] private IntVariable playthroughCount;

    [Header("Output Events")]
    [SerializeField] private ScriptableEventNoParam onUnlocksProcessed;

    // Hàm này sẽ được gọi bởi EventListener sau khi Master Decks đã được nạp.
    public void ProcessUnlocks()
    {
        foreach (var rule in allUnlockRules)
        {
            if (AreConditionsMet(rule))
            {
                ExecuteActions(rule);
            }
        }

        if (onUnlocksProcessed != null)
        {
            onUnlocksProcessed.Raise();
        }
    }

    private bool AreConditionsMet(UnlockRule rule)
    {
        // Kiểm tra điều kiện số lượt chơi
        if (playthroughCount.Value < rule.requiredPlaythroughs)
        {
            return false;
        }

        // Kiểm tra tất cả các cờ trạng thái
        if (rule.requiredFlags.Any(flag => flag == null || flag.Value == false))
        {
            return false;
        }

        return true;
    }

    private void ExecuteActions(UnlockRule rule)
    {
        if (rule.targetDeck == null || rule.cardsToAdd == null) return;

        foreach (var card in rule.cardsToAdd)
        {
            if (card != null && !rule.targetDeck.Contains(card))
            {
                rule.targetDeck.Add(card);
            }
        }
    }
}