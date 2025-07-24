using UnityEngine;
using System;
using System.Collections;

public class SecondCard : MonoBehaviour
{
    private SwipeEffect _firstCardSwipeEffect;
    private GameObject _firstCardGameObject;

    public static event Action<SwipeEffect> OnSecondCardBecomesActiveSwipeEffect;

    // LƯU Ý: Ở phiên bản code này, các trường cardFront/cardBack VÀ logic FlipAnimation chưa có.
    // public float flipDuration = 0.3f;
    // public GameObject cardFront;
    // public GameObject cardBack;

    public void SetFirstCardToObserve(GameObject firstCardGameObject, SwipeEffect firstCardSwipeEffect)
    {
        if (firstCardGameObject == null || firstCardSwipeEffect == null)
        {
            Debug.LogError("SecondCard: Cannot set first card. Received null reference for GameObject or SwipeEffect.", this);
            return;
        }

        if (_firstCardSwipeEffect != null)
        {
            _firstCardSwipeEffect.cardDestroyed -= OnFirstCardDestroyed;
        }

        _firstCardGameObject = firstCardGameObject;
        _firstCardSwipeEffect = firstCardSwipeEffect;

        _firstCardSwipeEffect.cardDestroyed += OnFirstCardDestroyed;

        // KHI THẺ NÀY LÀ SECOND CARD: Ở phiên bản này, nó không bật/tắt mặt thẻ.
        // if (cardFront != null) cardFront.SetActive(false);
        // if (cardBack != null) cardBack.SetActive(true);
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    void Start()
    {
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        // Ở phiên bản này, không có kiểm tra cardFront/cardBack null.
        // if (cardFront == null || cardBack == null) { ... }
    }

    void Update()
    {
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
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }
    }

    void OnFirstCardDestroyed()
    {
        if (_firstCardSwipeEffect != null)
        {
            _firstCardSwipeEffect.cardDestroyed -= OnFirstCardDestroyed;
            _firstCardSwipeEffect = null;
            _firstCardGameObject = null;
        }

        if (this.gameObject != null)
        {
            SwipeEffect newSwipeEffect = gameObject.AddComponent<SwipeEffect>();

            OnSecondCardBecomesActiveSwipeEffect?.Invoke(newSwipeEffect);

            // Ở phiên bản code này, không có hiệu ứng xoay lật.
            // StartCoroutine(FlipAndScaleRoutine());

            Destroy(this);
        }
        else
        {
            Debug.LogWarning("SecondCard: GameObject was already destroyed before OnFirstCardDestroyed could add SwipeEffect.");
        }
    }

    // Ở phiên bản code này, không có Coroutine FlipAndScaleRoutine.
    // private IEnumerator FlipAndScaleRoutine() { ... }

    void OnDestroy()
    {
        if (_firstCardSwipeEffect != null)
        {
            _firstCardSwipeEffect.cardDestroyed -= OnFirstCardDestroyed;
        }
    }
}