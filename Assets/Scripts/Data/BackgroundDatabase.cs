using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BackgroundInfo
{
    public string backgroundName; 
    public Sprite backgroundSprite;
    public string displayName;
}

[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "NguoiKienTao/Background Database")]
public class BackgroundDatabase : ScriptableObject
{
    public List<BackgroundInfo> allBackgrounds;
}