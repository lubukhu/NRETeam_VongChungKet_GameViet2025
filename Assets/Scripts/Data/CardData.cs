using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Card/Card Data")]
public class CardData : ScriptableObject
{
    [Header("General Card Information")]
    public string cardID;
    public string characterName;
    public Sprite characterSprite;

    [TextArea(3, 10)]
    public string dialogueText;

    [Header("Left Choice")]
    [TextArea(1, 5)]
    public string leftChoiceText;
    public List<CardEffect> leftChoiceEffects;

    [Header("Right Choice")]
    [TextArea(1, 5)]
    public string rightChoiceText;
    public List<CardEffect> rightChoiceEffects;

    [Header("Next Card Logic (Optional)")]
    public string nextCardIDAfterLeftChoice;
    public string nextCardIDAfterRightChoice;
}

public enum StatType
{
    Finance,
    Trust,
    Environment,
    Culture
}

[System.Serializable]
public class CardEffect
{
    public StatType statType;
    public int changeAmount;
    [TextArea(1, 3)]
    public string effectDescription;
}