// [tooltips] Xử lý input kéo/thả, quyết định kết quả (vuốt hoặc trả về) và phát ra các sự kiện tương ứng.
using UnityEngine;
using UnityEngine.EventSystems;
using Obvious.Soap;

public class CardInteraction : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Drag Config")]
    [Tooltip("Thẻ phải được kéo bao nhiêu phần trăm chiều rộng của Vùng Tương Tác để được tính là một cú vuốt.")]
    [SerializeField, Range(0.1f, 1f)] private float swipeThresholdPercentage = 0.4f;

    [Header("Output Events")]
    [SerializeField] private ScriptableEventBool onCardSwiped;
    [SerializeField] private ScriptableEventFloat onCardDragProgress;
    [SerializeField] private ScriptableEventNoParam onCardReturn; 
    [SerializeField] private ScriptableEventNoParam onCardInteractionStart;
    private RectTransform _rectTransform;
    private Vector2 _initialPosition;
    private RectTransform _swipeAreaContainer; // Biến để lưu trữ vùng tương tác tìm được

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        GameObject containerObject = GameObject.FindWithTag("SwipeAreaContainer");
        if (containerObject != null)
        {
            _swipeAreaContainer = containerObject.GetComponent<RectTransform>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onCardInteractionStart != null)
        {
            onCardInteractionStart.Raise();
        }

        _initialPosition = _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = _rectTransform.anchoredPosition + eventData.delta;
        Rect containerRect = _swipeAreaContainer.rect;
        Rect cardRect = _rectTransform.rect;

        // Tính toán tọa độ biên (min/max) cuả border
        float minX = containerRect.xMin + (cardRect.width * 2.5f);
        float maxX = containerRect.xMax - (cardRect.width * 2.5f);
        float minY = containerRect.yMin + (cardRect.height * 4.5f);
        float maxY = containerRect.yMax - (cardRect.height * 4.5f);
        
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        _rectTransform.anchoredPosition = newPos;

        float referenceWidth = _swipeAreaContainer.rect.width / 2f;
        float progress = Mathf.Clamp((_rectTransform.anchoredPosition.x - _initialPosition.x) / referenceWidth, -1f, 1f);
        
        if (onCardDragProgress != null)
        {
            onCardDragProgress.Raise(progress);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onCardDragProgress != null)
        {
            onCardDragProgress.Raise(0);
        }

        float swipeThreshold = _swipeAreaContainer.rect.width * swipeThresholdPercentage;
        float distanceMoved = Mathf.Abs(_rectTransform.anchoredPosition.x - _initialPosition.x);

        if (distanceMoved < swipeThreshold)
        {
            if (onCardReturn != null)
            {
                onCardReturn.Raise();
            }
        }
        else
        {
            bool isRightSwipe = _rectTransform.anchoredPosition.x > _initialPosition.x;
            if (onCardSwiped != null)
            {
                onCardSwiped.Raise(isRightSwipe);
            }
        }
    }
}