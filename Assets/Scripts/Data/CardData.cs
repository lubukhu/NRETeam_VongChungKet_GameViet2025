// [tooltips] Định nghĩa cấu trúc dữ liệu cho một thẻ bài, bao gồm hành vi, tần suất xuất hiện, và các hệ quả khi người chơi lựa chọn.
using UnityEngine;
using System.Collections.Generic;
using Obvious.Soap;

#region Enums & Data Structures

public enum StatType
{
    Finance,
    Trust,
    Environment,
    Culture,
    Tutorial
}

public enum CardBehaviorType
{
    Decision,
    Narrative
}

public enum CardFrequency
{
    Standard,
    OncePerPlaythrough
}

public enum NarrativeEffectType
{
    AddCardToDeck,
    RemoveCardFromDeck,
    SetNarrativeFlag,
}

[System.Serializable]
public class CardEffect
{
    public StatType statType;
    public int changeAmount;
}

[System.Serializable]
public class NarrativeEffect
{
    public NarrativeEffectType effectType;
    public CardData targetCard;
    public ScriptableListCardData targetDeck;
    public BoolVariable narrativeFlag;
    public bool flagValue;
}

[System.Serializable]
public class ChoiceResult
{
    [TextArea(1, 5)]
    public string choiceText;
    public List<CardEffect> statEffects;
    public List<NarrativeEffect> narrativeEffects;
}

#endregion

[CreateAssetMenu(fileName = "NewCardData", menuName = "NguoiKienTao/Card Data")]
public class CardData : ScriptableObject
{
    [Header("General Card Information")]
    public string cardID;
    public string characterName;
    public Sprite characterSprite;

    [TextArea(3, 10)]
    public string dialogueText;

    [Header("Card Category")]
    public StatType cardCategory;

    [Header("Behavior & Lifecycle")]
    public CardBehaviorType behaviorType = CardBehaviorType.Decision;
    public CardFrequency frequency = CardFrequency.Standard;

    [Header("Choices")]
    [Tooltip("Cấu hình cho lựa chọn bên trái (hoặc lựa chọn duy nhất cho thẻ Narrative)")]
    public ChoiceResult leftChoice;

    [Tooltip("Cấu hình cho lựa chọn bên phải. Sẽ bị bỏ qua nếu Behavior Type là Narrative.")]
    public ChoiceResult rightChoice;
}