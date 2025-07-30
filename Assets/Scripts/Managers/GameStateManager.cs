// [tooltips] Lắng nghe các chỉ số, kiểm tra các điều kiện kết thúc game và thông báo.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    [Header("Ending Definitions")]
    [Tooltip("Danh sách tất cả các điều kiện có thể kết thúc game.")]
    [SerializeField] private List<EndingCondition> endingConditions;

    [Header("Output Events")]
    [SerializeField] private ScriptableEventString onGameEnded;

    private bool _isGameOver = false;

    private void OnEnable()
    {
        _isGameOver = false;
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
            if (condition.comparison == ComparisonType.LessThanOrEqual)
            {
                if (condition.statToWatch.Value <= condition.threshold)
                {
                    conditionMet = true;
                }
            }
            else if (condition.comparison == ComparisonType.GreaterThanOrEqual)
            {
                if (condition.statToWatch.Value >= condition.threshold)
                {
                    conditionMet = true;
                }
            }

            if (conditionMet)
            {
                TriggerEnding(condition.endingId);
                break; 
            }
        }
    }

    private void TriggerEnding(string endingId)
    {
        _isGameOver = true;
        onGameEnded.Raise($"Game Over: {endingId}");
        Time.timeScale = 0;
    }
}