using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CharacterInfo
{
    public CharacterName characterEnum;
    public string displayName;
    public Sprite characterSprite;
}

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "NguoiKienTao/Database/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterInfo> allCharacters;
}