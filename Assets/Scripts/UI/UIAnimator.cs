// [tooltips] Quản lý tất cả các hiệu ứng của thẻ bài và UI tĩnh bằng DOTween.
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIAnimator : MonoBehaviour
{
    [Header("Static UI References")]
    [SerializeField] private TextMeshProUGUI leftChoiceText;
    [SerializeField] private TextMeshProUGUI rightChoiceText;
    
    [Header("Dependencies")]
    [SerializeField] private CardViewManager cardViewManager;

    [Header("Animation Config")]
    [SerializeField] private float flipDuration = 0.4f;
    [SerializeField] private float exitDuration = 0.3f;
    [SerializeField] private float returnDuration = 0.2f;

    public void UpdatePeekAnimation(float progress)
    {
        leftChoiceText.alpha = Mathf.Clamp01(progress * 2);
        rightChoiceText.alpha = Mathf.Clamp01(-progress * 2);
    }

    public void AnimateFlip(Transform cardTransform, System.Action onComplete)
    {
        cardTransform.DORotate(new Vector3(0, 0, 0), flipDuration)
            .From(new Vector3(0, 180, 0))
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void AnimateCardExit(Transform cardTransform, bool toTheRight, System.Action onComplete)
    {
        float exitX = (Screen.width / 2f + ((RectTransform)cardTransform).rect.width) * (toTheRight ? 1 : -1);
        cardTransform.DOMoveX(exitX, exitDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => onComplete?.Invoke());
    }
    
    // Sẽ được gọi bởi EventListener lắng nghe onCardReturn
    public void OnCardReturn()
    {
        Transform activeCardTransform = cardViewManager.GetActiveCardTransform();
        if (activeCardTransform != null)
        {
            activeCardTransform.DOLocalMove(Vector3.zero, returnDuration).SetEase(Ease.OutQuad);
        }
    }
}