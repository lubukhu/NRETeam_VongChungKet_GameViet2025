// [tooltips] Lắng nghe sự kiện kết thúc game để tăng bộ đếm lượt chơi.
using UnityEngine;
using Obvious.Soap;

public class GameSessionManager : MonoBehaviour
{
    [Tooltip("Biến lưu số lượt chơi đã hoàn thành.")]
    [SerializeField] private IntVariable playthroughCount;
    
    public void OnGameEnded(string eventData)
    {
        if (playthroughCount != null) playthroughCount.Value++;
        
        string originalId = eventData.Replace("Game Over: ", "").Trim();
        
        if (GalleryDataManager.Instance != null && !string.IsNullOrEmpty(originalId))
        {
            GalleryDataManager.Instance.UnlockEnding(originalId);
        }
    }
}