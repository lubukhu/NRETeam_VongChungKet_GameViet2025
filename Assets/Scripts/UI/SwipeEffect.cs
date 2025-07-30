// [tooltips] Xử lý tương tác vuốt thẻ và phát đi một ScriptableEvent khi hoàn tất.
using UnityEngine;
using UnityEngine.EventSystems;
using Obvious.Soap;

public class SwipeEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _initialPosition;
    private bool _isSwipingOrMoving;

    [Header("SOAP Events")]
    [SerializeField] private ScriptableEventBool onCardSwiped;

    public void OnDrag(PointerEventData eventData)
    {
        if (_isSwipingOrMoving) return;

        transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);

        if (transform.localPosition.x - _initialPosition.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, -30,
                (_initialPosition.x + transform.localPosition.x) / (Screen.width / 2f)));
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, 30,
                (_initialPosition.x - transform.localPosition.x) / (Screen.width / 2f)));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isSwipingOrMoving) return;
        _initialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isSwipingOrMoving) return;

        float distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
        if (distanceMoved < 0.3f * Screen.width)
        {
            transform.localPosition = _initialPosition;
            transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            bool isRightSwipe = transform.localPosition.x > _initialPosition.x;
            
            if (onCardSwiped != null)
            {
                onCardSwiped.Raise(isRightSwipe);
            }
            
            // Tắt GameObject ngay lập tức để CardViewManager có thể tái sử dụng
            gameObject.SetActive(false);
        }
    }
}