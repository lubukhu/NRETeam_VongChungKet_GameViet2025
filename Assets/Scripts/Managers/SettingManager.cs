using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Obvious.Soap;

public class SettingsManager : MonoBehaviour
{
    
    [SerializeField] private GameObject settingsPanel;
    [Header("Audio References")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private FloatVariable bgmVolume;
    [SerializeField] private FloatVariable sfxVolume;
    [SerializeField] private AudioClip buttonClickSfx;
    [Header("UI References")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        InitializeSlider(bgmSlider, bgmVolume);
        SetMusicVolume(bgmVolume.Value);

        InitializeSlider(sfxSlider, sfxVolume);
        SetSfxVolume(sfxVolume.Value);

        bgmSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void InitializeSlider(Slider slider, FloatVariable variable)
    {
        if (slider != null && variable != null)
        {
            slider.value = variable.Value;
        }
    }

    // --- HÀM ĐÃ ĐƯỢC SỬA LỖI ---
    public void SetMusicVolume(float value)
    {
        // Kẹp giá trị để đảm bảo nó luôn lớn hơn 0
        float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);

        if (bgmVolume != null) bgmVolume.Value = clampedValue;
        mainMixer.SetFloat("BGM_Volume", Mathf.Log10(clampedValue) * 20);
    }

    // --- HÀM ĐÃ ĐƯỢC SỬA LỖI ---
    public void SetSfxVolume(float value)
    {
        // Kẹp giá trị để đảm bảo nó luôn lớn hơn 0
        float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);

        if (sfxVolume != null) sfxVolume.Value = clampedValue;
        mainMixer.SetFloat("SFX_Volume", Mathf.Log10(clampedValue) * 20);
    }
    
    public void PlayButtonClickSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySfx(buttonClickSfx);
        }
    }
    
    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            // Bật/tắt Panel Settings
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
    
    public void ExitGame()
    {
        Debug.Log("Thoát game!");
        // Lệnh Application.Quit() chỉ hoạt động trong bản build, không có tác dụng trong Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}