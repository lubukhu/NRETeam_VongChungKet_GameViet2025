// [tooltips] Lắng nghe các chỉ số, kiểm tra các điều kiện kết thúc game và thông báo.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;
using System.Linq;

public class GameStateManager : MonoBehaviour
{
    [Header("Ending Definitions")]
    [Tooltip("Danh sách tất cả các điều kiện có thể kết thúc game.")]
    [SerializeField] private List<EndingCondition> endingConditions;
    [Tooltip("Tham chiếu đến biến tổng số ngày công tác.")]
    [SerializeField] private IntVariable totalDaysWorked; // Chỉ cần tham chiếu này

    
    [Header("Output Events")]
    [SerializeField] private ScriptableEventString onGameEnded;
    
    [Header("State Outputs")]
    [SerializeField] private StringVariable triggeredEndingID;
    
    [Tooltip("chỉ số curentDay tham chếu tới totalDayWorked!")]
    [SerializeField] private IntVariable currentDay;
    [SerializeField] private IntVariable endDay;   
    
    private bool _isGameOver = false;

    private void OnEnable()
    {
        _isGameOver = false;
        if (triggeredEndingID != null)
        {
            triggeredEndingID.Value = "";
        }
        
        foreach (var condition in endingConditions)
        {
            if (condition.statToWatch != null)
            {
                condition.statToWatch.OnValueChanged += OnStatChanged;
            }
        }
    }

    private void OnDisable()
    {
        foreach (var condition in endingConditions)
        {
            if (condition.statToWatch != null)
            {
                condition.statToWatch.OnValueChanged -= OnStatChanged;
            }
        }
    }

    private void OnStatChanged(float _)
    {
        CheckAllEndConditions();
    }

    private void CheckAllEndConditions()
    {
        if (_isGameOver) return;

        foreach (var condition in endingConditions)
        {
            bool conditionMet = false;
            
            // Dùng switch-case để xử lý các loại điều kiện khác nhau
            switch (condition.checkType)
            {
                case ConditionCheckType.StatThreshold:
                    conditionMet = CheckStatThreshold(condition);
                    break;
                case ConditionCheckType.TimeAndBalance:
                    conditionMet = CheckTimeAndBalance(condition);
                    break;
            }

            if (conditionMet)
            {
                TriggerEnding(condition.endingId);
                return; // Tìm thấy một ending, thoát khỏi hàm
            }
        }
    }

    private bool CheckStatThreshold(EndingCondition condition)
    {
        if (condition.statToWatch == null) return false;
        if (condition.comparison == ComparisonType.LessThanOrEqual && condition.statToWatch.Value <= condition.threshold) return true;
        if (condition.comparison == ComparisonType.GreaterThanOrEqual && condition.statToWatch.Value >= condition.threshold) return true;
        return false;
    }

    private bool CheckTimeAndBalance(EndingCondition condition)
    {
        // Chỉ kiểm tra khi thời gian đã đủ
        if (totalDaysWorked.Value < condition.requiredDays) return false;

        // Kiểm tra xem tất cả các chỉ số trong danh sách có cân bằng không
        // .All() sẽ trả về true chỉ khi TẤT CẢ các phần tử đều thỏa mãn điều kiện
        return condition.statsToBalance.All(stat => 
            stat.Value > condition.minBalanceThreshold && 
            stat.Value < condition.maxBalanceThreshold);
    }
    
    
    private void TriggerEnding(string endingId)
    {
        _isGameOver = true;
        
        // 1. Đặt ID của ending vào "hộp thư" cho NarrativeDirector
        if (triggeredEndingID != null)
        {
            triggeredEndingID.Value = endingId;
        }

        // 2. Lưu lại ngày kết thúc để truyền cho EndingScene
        if (currentDay != null && endDay != null)
        {
            endDay.Value = currentDay.Value;
        }
        
        onGameEnded.Raise($"Game Over: {endingId}");
    }
}