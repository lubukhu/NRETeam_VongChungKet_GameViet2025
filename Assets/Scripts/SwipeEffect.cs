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
    private bool _isSwipingOrMoving;

    public event Action cardMoved;
    public event Action cardDestroyed;
    public event Action<bool> cardSwiped;

    private Image _cardImage;

    void Awake()
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

        _distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
        if (_distanceMoved < 0.3f * Screen.width)
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

            cardSwiped?.Invoke(!_swipeLeft);

            StartCoroutine(MovedCard());
        }
    }

    private IEnumerator ResetPosition()
    {
        float time = 0;
        float duration = 0.15f;
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
        _isSwipingOrMoving = false;
    }

    private IEnumerator MovedCard()
    {
        float time = 0;
        if (_cardImage == null)
        {
            Debug.LogError("SwipeEffect: _cardImage is null in MovedCard coroutine. Cannot animate fade.", this);
            Destroy(gameObject);
            yield break;
        }

        Color initialColor = _cardImage.color;
        Vector3 currentLocalPosition = transform.localPosition;

        float fadeDuration = 0.1f;
        float moveDistanceMultiplier = 3f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / fadeDuration;

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

        _cardImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

        cardDestroyed?.Invoke();

        yield return null;

        Destroy(gameObject);
    }
}