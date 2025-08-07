// [tooltips] Quản lý, lưu và tải dữ liệu tiến trình của người chơi (gallery unlocks).
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GalleryDataManager : MonoBehaviour
{
    public static GalleryDataManager Instance { get; private set; }
    
    public List<CharacterName> UnlockedCharacters { get; private set; } = new List<CharacterName>();
    public List<string> UnlockedEndingIDs { get; private set; } = new List<string>();
    public List<string> UnlockedBackgroundNames { get; private set; } = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData(); 
    }
    

    public void UnlockCharacter(CharacterName character)
    {
        if (!UnlockedCharacters.Contains(character))
        {
            UnlockedCharacters.Add(character);
            Debug.Log($"<color=yellow>Đã mở khóa Nhân vật: {character}</color>");
            SaveData();
        }
    }

    public void UnlockEnding(string endingId)
    {
        if (!UnlockedEndingIDs.Contains(endingId))
        {
            UnlockedEndingIDs.Add(endingId);
            Debug.Log($"<color=yellow>Đã mở khóa Ending: {endingId}</color>");
            SaveData();
        }
    }
    
    public void UnlockBackground(string backgroundName)
    {
        if (!string.IsNullOrEmpty(backgroundName) && !UnlockedBackgroundNames.Contains(backgroundName))
        {
            UnlockedBackgroundNames.Add(backgroundName);
            Debug.Log($"<color=yellow>Đã mở khóa Background: {backgroundName}</color>");
            SaveData();
        }
    }
    


    public void SaveData()
    {
        string charactersString = string.Join(",", UnlockedCharacters.Select(c => (int)c));
        PlayerPrefs.SetString("UnlockedCharacters", charactersString);

        string endingsString = string.Join(",", UnlockedEndingIDs);
        PlayerPrefs.SetString("UnlockedEndings", endingsString);

        string backgroundsString = string.Join(",", UnlockedBackgroundNames);
        PlayerPrefs.SetString("UnlockedBackgrounds", backgroundsString);

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        string charactersString = PlayerPrefs.GetString("UnlockedCharacters", "");
        if (!string.IsNullOrEmpty(charactersString))
        {
            // Đọc chuỗi số, chuyển thành số nguyên, rồi mới ép kiểu về enum
            UnlockedCharacters = charactersString.Split(',')
                .Select(s => (CharacterName)int.Parse(s))
                .ToList();
        }

        string endingsString = PlayerPrefs.GetString("UnlockedEndings", "");
        if (!string.IsNullOrEmpty(endingsString))
        {
            UnlockedEndingIDs = endingsString.Split(',').ToList();
        }

        string backgroundsString = PlayerPrefs.GetString("UnlockedBackgrounds", "");
        if (!string.IsNullOrEmpty(backgroundsString))
        {
            UnlockedBackgroundNames = backgroundsString.Split(',').ToList();
        }
    }
    
    public void ResetGalleryData()
    {
        PlayerPrefs.DeleteKey("UnlockedCharacters");
        PlayerPrefs.DeleteKey("UnlockedEndings");
        PlayerPrefs.DeleteKey("UnlockedBackgrounds");
        UnlockedCharacters.Clear();
        UnlockedEndingIDs.Clear();
        UnlockedBackgroundNames.Clear();
        Debug.Log("<color=orange>Dữ liệu Gallery đã được reset!</color>");
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

}