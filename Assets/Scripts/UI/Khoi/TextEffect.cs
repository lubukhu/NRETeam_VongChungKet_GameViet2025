using System.Collections;
using UnityEngine;
using TMPro;

public class TextEffect : MonoBehaviour
{ 
    public TextMeshProUGUI textMeshPro;

    // Thời gian của hiệu ứng (mờ/rõ, to/nhỏ)
    public float effectDuration = 2f;

    // Kích thước font nhỏ nhất và lớn nhất
    public float minFontSize = 36f;
    public float maxFontSize = 72f;

    // Độ mờ/rõ (alpha) nhỏ nhất và lớn nhất
    public float minAlpha = 0f;
    public float maxAlpha = 1f;

    // Bắt đầu hiệu ứng khi game chạy
    void Start()
    {
        // Kiểm tra xem đã gán TextMeshPro chưa
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
            if (textMeshPro == null)
            {
                return;
            }
        }
        // Bắt đầu Coroutine để chạy hiệu ứng
        StartCoroutine(AnimateText());
    }
    
    private IEnumerator AnimateText()
    {
        while (true) // Lặp lại hiệu ứng vô hạn
        {
            // Hiệu ứng tăng kích thước và làm rõ chữ
            yield return StartCoroutine(LerpText(minFontSize, maxFontSize, minAlpha, maxAlpha, effectDuration));

            // Hiệu ứng giảm kích thước và làm mờ chữ
            yield return StartCoroutine(LerpText(maxFontSize, minFontSize, maxAlpha, minAlpha, effectDuration));
        }
    }

    private IEnumerator LerpText(float startSize, float endSize, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            // Tính toán giá trị nội suy (interpolation)
            timer += Time.deltaTime;
            float t = timer / duration;

            // Dùng Mathf.SmoothStep để tạo hiệu ứng mượt mà hơn
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Cập nhật kích thước font
            textMeshPro.fontSize = Mathf.Lerp(startSize, endSize, smoothT);

            // Cập nhật độ trong suốt (alpha)
            Color newColor = textMeshPro.color;
            newColor.a = Mathf.Lerp(startAlpha, endAlpha, smoothT);
            textMeshPro.color = newColor;

            yield return null; // Chờ đến frame tiếp theo
        }
    }
}