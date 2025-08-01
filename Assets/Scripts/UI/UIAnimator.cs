// [tooltips] Quản lý tất cả các hiệu ứng của thẻ bài và UI tĩnh bằng DOTween.
using UnityEngine;
using UnityEngine.UI; // Thêm vào để dùng Image
using DG.Tweening;
using TMPro;
using Obvious.Soap;

public class UIAnimator : MonoBehaviour
{
    [Header("Static UI References")]
    [SerializeField] private CanvasGroup leftDialogueBoxCanvasGroup;
    [SerializeField] private CanvasGroup rightDialogueBoxCanvasGroup;

    [Header("Game State Inputs")]
    [SerializeField] private GameObjectVariable currentDisplayedCardGameObject;

    [Header("Animation Config")]
    [SerializeField] private float cardRotationFactor = 20f; // Góc nghiêng tối đa
    [SerializeField] private float returnDuration = 0.3f;    // Thời gian quay về
    [SerializeField] private float flipDuration = 2f;
    
    public void ForceResetCardState()
    {
        if (currentDisplayedCardGameObject.Value != null)
        {
            Transform cardTransform = currentDisplayedCardGameObject.Value.transform;
            cardTransform.DOKill();

            // 2. Ép thẻ về vị trí và góc xoay ban đầu.
            cardTransform.localPosition = Vector3.zero;
            cardTransform.localRotation = Quaternion.identity;
        }
    }
    // Phương thức này được gọi bởi EventListener lắng nghe onCardDragProgress (FloatEvent)
    public void UpdateDragAnimation(float progress)
    {
        if (leftDialogueBoxCanvasGroup != null)
        {
            leftDialogueBoxCanvasGroup.alpha = Mathf.Clamp01(progress * 5f);
        }
        if (rightDialogueBoxCanvasGroup != null)
        {
            rightDialogueBoxCanvasGroup.alpha = Mathf.Clamp01(-progress * 5f);
        }

        // Thêm hiệu ứng XOAY thẻ khi kéo
        if (currentDisplayedCardGameObject.Value != null)
        {
            Transform cardTransform = currentDisplayedCardGameObject.Value.transform;
            float targetAngle = -progress * cardRotationFactor;
            cardTransform.localRotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }
    // Phương thức này được gọi bởi EventListener lắng nghe onCardReturn (NoParamEvent)
    public void AnimateCardReturn()
    {
        if (currentDisplayedCardGameObject.Value == null) return;

        Transform activeCardTransform = currentDisplayedCardGameObject.Value.transform;
        activeCardTransform.DOLocalMove(Vector3.zero, returnDuration).SetEase(Ease.OutQuad);
        activeCardTransform.DORotate(Vector3.zero, returnDuration).SetEase(Ease.OutQuad);
    }
    
    public void AnimateFlipCurrentCard() 
    {
        if (currentDisplayedCardGameObject != null && currentDisplayedCardGameObject.Value != null)
        {
            Transform cardTransform = currentDisplayedCardGameObject.Value.transform; // Lấy Transform từ GameObjectVariable
            cardTransform.localRotation = Quaternion.identity; 
            cardTransform.DORotate(new Vector3(0, 0, 0), flipDuration)
                .From(new Vector3(0, 180, 0))
                .SetEase(Ease.OutBack);
        } 
    }

}