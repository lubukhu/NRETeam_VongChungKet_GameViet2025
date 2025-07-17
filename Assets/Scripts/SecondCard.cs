using UnityEngine;
using System;

public class SecondCard : MonoBehaviour
{
    private SwipeEffect _firstCardSwipeEffect;
    private GameObject _firstCardGameObject;

    public static event Action<SwipeEffect> OnSecondCardBecomesActiveSwipeEffect;

    public void SetFirstCardToObserve(GameObject firstCardGameObject, SwipeEffect firstCardSwipeEffect)
    {
        if (firstCardGameObject == null || firstCardSwipeEffect == null)
        {
            Debug.LogError("SecondCard: Cannot set first card. Received null reference for GameObject or SwipeEffect.", this);
            return;
        }

        // Đảm bảo hủy đăng ký sự kiện từ thẻ cũ nếu đã có để tránh rò rỉ bộ nhớ
        // (Đây là logic tốt, giữ lại)
        if (_firstCardSwipeEffect != null)
        {
            _firstCardSwipeEffect.cardDestroyed -= OnFirstCardDestroyed;
        }

        _firstCardGameObject = firstCardGameObject;
        _firstCardSwipeEffect = firstCardSwipeEffect;

        _firstCardSwipeEffect.cardDestroyed += OnFirstCardDestroyed;

        // Debug.Log("SecondCard: Successfully set first card to observe: " + _firstCardGameObject.name);
    }

    void Start()
    {
        // Khởi tạo scale ban đầu
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    void Update()
    {
        // Kiểm tra _firstCardGameObject null để tránh lỗi sau khi thẻ đầu tiên bị hủy
        if (_firstCardGameObject != null)
        {
            float distanceMoved = _firstCardGameObject.transform.localPosition.x;
            if (Mathf.Abs(distanceMoved) > 0.01f)
            {
                float normalizedDistance = Mathf.Clamp01(Mathf.Abs(distanceMoved) / (Screen.width / 2f));
                float step = Mathf.SmoothStep(0.8f, 1f, normalizedDistance);
                transform.localScale = new Vector3(step, step, step);
            }
            else if (transform.localScale.x != 0.8f)
            {
                // Đảm bảo trở về kích thước ban đầu nếu thẻ không di chuyển
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }
        // Nếu _firstCardGameObject là null, không làm gì cả
    }

    void OnFirstCardDestroyed()
    {
        // Debug.Log("SecondCard: OnFirstCardDestroyed called for " + gameObject.name); // Để kiểm tra

        // Hủy đăng ký sự kiện từ thẻ cũ để tránh rò rỉ bộ nhớ
        if (_firstCardSwipeEffect != null)
        {
            _firstCardSwipeEffect.cardDestroyed -= OnFirstCardDestroyed;
            _firstCardSwipeEffect = null; // Xóa tham chiếu ngay sau khi hủy đăng ký
            _firstCardGameObject = null; // Xóa tham chiếu
        }

        // Thẻ này (SecondCard) bây giờ sẽ trở thành thẻ chính
        // Đảm bảo gameObject chưa bị hủy trước khi AddComponent
        if (this.gameObject != null) // Kiểm tra an toàn trước khi AddComponent
        {
            SwipeEffect newSwipeEffect = gameObject.AddComponent<SwipeEffect>();

            // Thông báo cho Instantiator rằng thẻ này đã trở thành SwipeEffect mới
            OnSecondCardBecomesActiveSwipeEffect?.Invoke(newSwipeEffect);

            // Hủy bỏ component SecondCard vì nó không còn cần thiết
            // Debug.Log("SecondCard: Destroying SecondCard component on " + gameObject.name); // Để kiểm tra
            Destroy(this);
        }
        else
        {
            Debug.LogWarning("SecondCard: GameObject was already destroyed before OnFirstCardDestroyed could add SwipeEffect.");
        }
    }

    // Đảm bảo hủy đăng ký khi SecondCard bị hủy
    void OnDestroy()
    {
        if (_firstCardSwipeEffect != null)
        {
            _firstCardSwipeEffect.cardDestroyed -= OnFirstCardDestroyed;
        }
    }
}