using UnityEngine;
using System.Collections.Generic;

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

    public string GetEndingMessage(string endingId)
    {
        foreach (var ending in badEndings)
        {
            if (ending.id == endingId)
            {
                return ending.message;
            }
        }
        Debug.LogWarning($"GameEndingsConfig: No ending message found for ID: {endingId}");
        return "Game Ended Unexpectedly.";
    }
}