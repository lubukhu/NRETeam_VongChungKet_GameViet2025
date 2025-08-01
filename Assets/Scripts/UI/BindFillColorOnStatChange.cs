using UnityEngine;
using UnityEngine.UI;
using Obvious.Soap;
using DG.Tweening; // THÊM DÒNG NÀY ĐỂ SỬ DỤNG DOTWEEN

[RequireComponent(typeof(Image))]
public class BindFillColorOnStatChange : MonoBehaviour
{
    [Header("Data Binding")]
    [Tooltip("Kéo ScriptableVariable của chỉ số cần theo dõi vào đây.")]
    [SerializeField] private FloatVariable statToWatch;
    [Tooltip("Kéo ScriptableVariable chứa giá trị tối đa của chỉ số (100).")]
    [SerializeField] private FloatVariable maxStatValue;

    [Header("Color Config")]
    [SerializeField] private Color increaseColor = new Color(0.45f, 0.85f, 0.45f); // Xanh lá
    [SerializeField] private Color decreaseColor = new Color(0.9f, 0.4f, 0.4f); // Đỏ
    [Tooltip("Thời gian (giây) để màu Xanh/Đỏ mờ dần về màu gốc.")]
    [SerializeField] private float colorFadeDuration = 0.5f; // THÊM THUỘC TÍNH MỚI

    private Color _neutralColor; 
    private Image _targetImage;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
        _neutralColor = _targetImage.color;
    }

    private void OnEnable()
    {
        if (statToWatch == null || maxStatValue == null) return;
        
        statToWatch.OnValueChanged += OnStatChanged;
        UpdateUI(statToWatch.Value, statToWatch.Value); 
    }

    private void OnDisable()
    {
        if (statToWatch == null) return;
        statToWatch.OnValueChanged -= OnStatChanged;
    }

    private void OnStatChanged(float newValue)
    {
        UpdateUI(newValue, statToWatch.PreviousValue);
    }
    
    private void UpdateUI(float currentValue, float previousValue)
    {
        if (maxStatValue.Value == 0) return;
        
        _targetImage.fillAmount = Mathf.Clamp01(currentValue / maxStatValue.Value);
        
        // --- LOGIC MỚI SỬ DỤNG DOTWEEN ---

        // 1. Dừng mọi animation màu đang chạy để tránh xung đột
        _targetImage.DOKill();

        // 2. Xác định màu cần "nháy"
        if (currentValue > previousValue)
        {
            _targetImage.color = increaseColor;
            // Lập tức gọi animation để mờ dần về màu gốc
            _targetImage.DOColor(_neutralColor, colorFadeDuration);
        }
        else if (currentValue < previousValue)
        {
            _targetImage.color = decreaseColor;
            // Lập tức gọi animation để mờ dần về màu gốc
            _targetImage.DOColor(_neutralColor, colorFadeDuration);
        }
        else // Lúc khởi tạo, chỉ set màu gốc
        {
            _targetImage.color = _neutralColor;
        }
    }
}