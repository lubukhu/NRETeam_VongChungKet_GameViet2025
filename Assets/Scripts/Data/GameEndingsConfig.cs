using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Thêm dòng này để sử dụng LINQ

[CreateAssetMenu(fileName = "NewGameEndingsConfig", menuName = "Game Config/Game Endings Config")]
public class GameEndingsConfig : ScriptableObject
{
    [System.Serializable]
    public class EndingMessage
    {
        public string id;
        [TextArea(3, 10)]
        public string message;
    }

    public List<EndingMessage> badEndings = new List<EndingMessage>();
    public List<EndingMessage> goodEndings = new List<EndingMessage>(); // Thêm danh sách cho các kết thúc tốt

    public string GetEndingMessage(string endingId)
    {
        // Tìm trong badEndings trước
        EndingMessage foundEnding = badEndings.FirstOrDefault(e => e.id == endingId);
        if (foundEnding != null)
        {
            return foundEnding.message;
        }

        // Nếu không tìm thấy trong badEndings, tìm trong goodEndings
        foundEnding = goodEndings.FirstOrDefault(e => e.id == endingId);
        if (foundEnding != null)
        {
            return foundEnding.message;
        }

        Debug.LogWarning($"GameEndingsConfig: No ending message found for ID: {endingId}");
        return "Game Ended Unexpectedly."; // Tin nhắn mặc định nếu không tìm thấy ID
    }
}