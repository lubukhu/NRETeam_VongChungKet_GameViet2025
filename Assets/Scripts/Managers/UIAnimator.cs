// [tooltips] Quản lý tất cả các hiệu ứng của thẻ bài và UI tĩnh bằng DOTween.
using UnityEngine;
using UnityEngine.UI; // Thêm vào để dùng Image
using DG.Tweening;
using TMPro;
using Obvious.Soap;
using System.Collections.Generic;

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
    
    [Header("Stat Preview Dots")]
    [Tooltip("Kéo các GameObject chấm tròn tương ứng vào đây")]
    [SerializeField] private GameObject financeDot;
    [SerializeField] private GameObject trustDot;
    [SerializeField] private GameObject environmentDot;
    [SerializeField] private GameObject cultureDot;
    
    [Header("Game State Inputs")]
    [Tooltip("Tham chiếu đến ScriptableVariable chứa CardData của thẻ hiện tại")]
    [SerializeField] private CardDataVariable currentActiveCard;
    
    [Header("Dot Animation Config")]
    [SerializeField] private float baseDotSize = 0.1f;
    [SerializeField] private float dotSizeMultiplier = 0.01f;
    
    // Dictionary để dễ dàng truy cập dot từ StatType
    private Dictionary<StatType, GameObject> _statDotMap;
   // Nghe sự kiên OnCardInteractionStart
   private void Awake()
   {
       // Khởi tạo map để truy cập nhanh
       _statDotMap = new Dictionary<StatType, GameObject>
       {
           { StatType.Finance, financeDot },
           { StatType.Trust, trustDot },
           { StatType.Environment, environmentDot },
           { StatType.Culture, cultureDot }
       };
   }
   
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
        
        // 1. Ẩn tất cả các chấm đi trước khi xử lý
        foreach (var dot in _statDotMap.Values)
        {
            if (dot != null) dot.SetActive(false);
        }

        // Nếu không có thẻ hoặc thẻ đang ở giữa, không làm gì cả
        if (currentActiveCard.Value == null || Mathf.Approximately(progress, 0))
        {
            return;
        }

        // 2. Xác định lựa chọn đang được xem trước (trái hay phải)
        ChoiceResult previewedChoice = progress > 0 ? currentActiveCard.Value.leftChoice : currentActiveCard.Value.rightChoice;

        // 3. Hiển thị và cập nhật các chấm tương ứng
        if (previewedChoice.statEffects != null)
        {
            foreach (var effect in previewedChoice.statEffects)
            {
                // Tìm chấm tròn tương ứng với chỉ số bị ảnh hưởng
                if (_statDotMap.TryGetValue(effect.statType, out GameObject dot))
                {
                    if (dot == null) continue;
                    
                    dot.SetActive(true);
                    dot.GetComponent<CanvasGroup>().alpha = Mathf.Abs(progress);
                    float newSize = baseDotSize + (Mathf.Abs(effect.changeAmount) * dotSizeMultiplier);
                    dot.transform.localScale = new Vector3(newSize, newSize, 1f);
                }
            }
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