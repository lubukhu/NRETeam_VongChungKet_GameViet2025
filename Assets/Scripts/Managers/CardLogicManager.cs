// [tooltips] Xử lý logic khi người chơi vuốt thẻ, áp dụng các hiệu ứng lên chỉ số và hệ thống tường thuật.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;
using System.Linq;

public class CardLogicManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private FloatVariable financeStat;
    [SerializeField] private FloatVariable trustStat;
    [SerializeField] private FloatVariable environmentStat;
    [SerializeField] private FloatVariable cultureStat;
    [SerializeField] private CardDataVariable currentCard;

    [Header("Ending Logic Inputs")]
    [Tooltip("Hộp thư chứa ID của ending vừa được GameStateManager kích hoạt.")]
    [SerializeField] private StringVariable triggeredEndingID;
    [Tooltip("Bộ bài chứa tất cả các thẻ ending.")]
    [SerializeField] private ScriptableListCardData masterDeck_Endings;
    
    [Header("Narrative Control")]
    [Tooltip("Hàng đợi chứa các thẻ bài được ép phải xuất hiện tiếp theo.")]
    [SerializeField] private ScriptableListCardData forcedCardsQueue;
    
    [Header("Dependencies")]
    [SerializeField] private DeckManager deckManager;
    
    [Header("Ending Events")]
    [SerializeField] private ScriptableEventNoParam onLoadEndingScene;
    [SerializeField] private ScriptableListCardData currentCardPool;
    [SerializeField] private ScriptableEventNoParam onNewPoolRequested;

    public void OnCardSwiped(bool isRightSwipe)
    {
        if (currentCard.Value == null) return;

        ChoiceResult choice = isRightSwipe ? currentCard.Value.leftChoice : currentCard.Value.rightChoice;

        // Apply stat effects
        foreach (var effect in choice.statEffects)
        {
            ApplyStatEffect(effect);
        }

        // Apply narrative effects
        foreach (var effect in choice.narrativeEffects)
        {
            ApplyNarrativeEffect(effect);
        }
        
        if (triggeredEndingID != null && !string.IsNullOrEmpty(triggeredEndingID.Value))
        {
            // Nếu CÓ, tìm thẻ ending và ra lệnh cho DeckManager hiển thị nó ngay lập tức.
            CardData endingCard = masterDeck_Endings.FirstOrDefault(c => c.cardID == triggeredEndingID.Value);
            if (endingCard != null)
            {
                deckManager.ForceSpawnSpecificCard(endingCard);
                triggeredEndingID.Value = ""; // Xóa thư sau khi đọc
            }
            else
            {
                Debug.LogError($"Ending triggered, but could not find card with ID: {triggeredEndingID.Value}");
            }
        }
        else
        {
            // 3. Nếu KHÔNG, tiếp tục luồng chơi game bình thường
            if (currentCardPool.Count > 0)
            {
                deckManager.DrawNextCard();
            }
            else
            {
                onNewPoolRequested.Raise();
            }
        }
    }

    private void ApplyStatEffect(CardEffect effect)
    {
        switch (effect.statType)
        {
            case StatType.Finance:
                financeStat.Value += effect.changeAmount;
                break;
            case StatType.Trust:
                trustStat.Value += effect.changeAmount;
                break;
            case StatType.Environment:
                environmentStat.Value += effect.changeAmount;
                break;
            case StatType.Culture:
                cultureStat.Value += effect.changeAmount;
                break;
        }
    }

    private void ApplyNarrativeEffect(NarrativeEffect effect)
    {
        switch (effect.effectType)
        {
            case NarrativeEffectType.AddCardToDeck:
                if (effect.targetDeck != null && effect.targetCard != null && !effect.targetDeck.Contains(effect.targetCard))
                    effect.targetDeck.Add(effect.targetCard);
                break;
            case NarrativeEffectType.RemoveCardFromDeck:
                 if (effect.targetDeck != null && effect.targetCard != null && effect.targetDeck.Contains(effect.targetCard))
                    effect.targetDeck.Remove(effect.targetCard);
                break;
            case NarrativeEffectType.SetNarrativeFlag:
                if (effect.narrativeFlag != null)
                    effect.narrativeFlag.Value = effect.flagValue;
                break;
            case NarrativeEffectType.TriggerEndingScreen:
                if (onLoadEndingScene != null)
                {
                    onLoadEndingScene.Raise();
                }
                break;
            case NarrativeEffectType.ForceNextCard:
                if (forcedCardsQueue != null && effect.targetCard != null)
                {
                    forcedCardsQueue.Insert(0, effect.targetCard);
                }
                break;
        }
    }
}