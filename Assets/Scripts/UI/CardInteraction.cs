// [tooltips] Xử lý input kéo/thả, quyết định kết quả (vuốt hoặc trả về) và phát ra các sự kiện tương ứng.
using UnityEngine;
using UnityEngine.EventSystems;
using Obvious.Soap;

public class CardInteraction : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Output Events")]
    [SerializeField] private ScriptableEventBool onCardSwiped;
    [SerializeField] private ScriptableEventFloat onCardDragProgress;
    [SerializeField] private ScriptableEventNoParam onCardReturn; 

    private RectTransform _rectTransform;
    private Vector2 _initialPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialPosition = _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta;

        float progress = Mathf.Clamp((_rectTransform.anchoredPosition.x - _initialPosition.x) / (Screen.width / 4f), -1f, 1f);
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

        float swipeThreshold = Screen.width * 0.25f;
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