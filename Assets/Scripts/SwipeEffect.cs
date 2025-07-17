using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _initialPosition;
    private float _distanceMoved;
    private bool _swipeLeft;
    private bool _isSwipingOrMoving; // Biến trạng thái để ngăn spam

    public event Action cardMoved; // Thẻ đã được vuốt đủ mạnh và bắt đầu di chuyển đi
    public event Action cardDestroyed; // Thẻ đã hoàn thành animation biến mất và bị hủy

    // Thêm một tham chiếu đến Image component để tránh GetComponent lặp lại
    private Image _cardImage;

    void Awake() // Sử dụng Awake để lấy tham chiếu sớm
    {
        _cardImage = GetComponent<Image>();
        if (_cardImage == null)
        {
            Debug.LogError("SwipeEffect: Image component not found on this GameObject!", this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isSwipingOrMoving) return;

        transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);

        if (transform.localPosition.x - _initialPosition.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, -30,
                (_initialPosition.x + transform.localPosition.x) / (Screen.width / 2f))); // Thêm 'f' cho float literal
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, 30,
                (_initialPosition.x - transform.localPosition.x) / (Screen.width / 2f))); // Thêm 'f' cho float literal
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

        _distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
        if (_distanceMoved < 0.3f * Screen.width) // Thêm 'f' cho float literal
        {
            StartCoroutine(ResetPosition());
        }
        else
        {
            if (transform.localPosition.x > _initialPosition.x)
            {
                _swipeLeft = false;
            }
            else
            {
                _swipeLeft = true;
            }
            _isSwipingOrMoving = true;
            cardMoved?.Invoke();
            StartCoroutine(MovedCard());
        }
    }

    private IEnumerator ResetPosition()
    {
        float time = 0;
        // Đặt một giá trị duration rõ ràng hơn
        float duration = 0.15f; // Thời gian reset vị trí
        Vector3 currentPos = transform.localPosition;
        Quaternion currentRot = transform.localRotation;
        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(currentPos, _initialPosition, time / duration);
            transform.localRotation = Quaternion.Slerp(currentRot, Quaternion.identity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _initialPosition;
        transform.localEulerAngles = Vector3.zero;
        _isSwipingOrMoving = false; // Reset trạng thái sau khi trở về
    }

    private IEnumerator MovedCard()
    {
        float time = 0;
        // Sử dụng _cardImage đã được cache
        Color initialColor = _cardImage.color;
        Vector3 currentLocalPosition = transform.localPosition;

        float fadeDuration = 0.1f; // Thời gian thẻ biến mất
        float moveDistanceMultiplier = 3f; // Khoảng cách thẻ di chuyển ra khỏi màn hình

        while (time < fadeDuration) // Sửa điều kiện vòng lặp dựa trên thời gian
        {
            time += Time.deltaTime;
            float normalizedTime = time / fadeDuration; // Giá trị từ 0 đến 1

            if (_swipeLeft)
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(currentLocalPosition.x,
                    currentLocalPosition.x - Screen.width * moveDistanceMultiplier, normalizedTime), transform.localPosition.y, 0);
            }
            else
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(currentLocalPosition.x,
                    currentLocalPosition.x + Screen.width * moveDistanceMultiplier, normalizedTime), transform.localPosition.y, 0);
            }
            _cardImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.SmoothStep(1, 0, normalizedTime));
            yield return null;
        }

        // Đảm bảo alpha về 0 tuyệt đối và vị trí cuối cùng
        _cardImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        // Có thể thêm một dòng để đưa thẻ ra xa hơn nữa nếu muốn

        // === THAY ĐỔI QUAN TRỌNG TẠI ĐÂY ===
        // Báo hiệu thẻ đã bị hủy (hoàn thành animation) TRƯỚC KHI hủy GameObject.
        // Điều này cho phép SecondCard xử lý chuyển đổi trước khi đối tượng bị hủy hoàn toàn.
        cardDestroyed?.Invoke();

        // Đợi một frame hoặc một khoảng thời gian nhỏ trước khi hủy
        // Điều này giúp đảm bảo tất cả các listener đã nhận và xử lý sự kiện
        yield return null; // Đợi 1 frame

        // Hủy GameObject của thẻ này
        Destroy(gameObject);
    }
}