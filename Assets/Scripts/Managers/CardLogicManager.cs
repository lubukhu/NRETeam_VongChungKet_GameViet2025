// [tooltips] Xử lý logic khi người chơi vuốt thẻ, áp dụng các hiệu ứng lên chỉ số và hệ thống tường thuật.
using UnityEngine;
using Obvious.Soap;

public class CardLogicManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private FloatVariable financeStat;
    [SerializeField] private FloatVariable trustStat;
    [SerializeField] private FloatVariable environmentStat;
    [SerializeField] private FloatVariable cultureStat;
    [SerializeField] private CardDataVariable currentCard;

    [Header("Dependencies")]
    [SerializeField] private DeckManager deckManager;

    public void OnCardSwiped(bool isRightSwipe)
    {
        if (currentCard.Value == null) return;
        Debug.Log("CurrentCard: " + currentCard.Value.name + "");

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
        if (deckManager != null && deckManager.gameObject.activeInHierarchy)
        {
            Debug.Log($"[KIỂM TRA] CardLogicManager đang gọi đến DeckManager tên là '{deckManager.gameObject.name}'. Click vào đây để xem nó.", deckManager.gameObject);
        }
        else
        {
            Debug.LogError("[LỖI] Tham chiếu deckManager bị null hoặc không hoạt động!");
            return; // Dừng lại nếu có lỗi
        }
        deckManager.DrawNextCard();
        Debug.Log("Card mới đc rút!");
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
        }
    }
}