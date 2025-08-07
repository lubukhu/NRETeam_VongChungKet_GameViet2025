using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Obvious.Soap;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingSceneController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject GalleryPanel;
    [Header("Data Inputs")]
    [SerializeField] private IntVariable endDay;
    [SerializeField] private string gameplaySceneName = "GameplayScene"; 
    [SerializeField] private string beginSceneName = "BeginScene";
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private float rewindSpeed = 0.007f;
    
    private const int START_DAY = 1;

    void Start()
    {
        // Hiển thị ngày kết thúc
        dateText.text = $"{endDay.Value}";

        // Bắt đầu coroutine tua ngược
        StartCoroutine(RewindDateRoutine());
    }

    private IEnumerator RewindDateRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        int displayDay = endDay.Value;
        float rewindAccumulator = 0f;

        while (displayDay > START_DAY)
        {
            //dateText.text = $"SỐ NGÀY CÔNG TÁC: {displayDay}";
            dateText.text = $"{displayDay}";
            rewindAccumulator += (1 / rewindSpeed * Time.deltaTime);

            if (rewindAccumulator >= 1f)
            {
                int daysToRewind = Mathf.FloorToInt(rewindAccumulator);
                displayDay -= daysToRewind;
                rewindAccumulator -= daysToRewind;
            }
            
            if (displayDay < START_DAY)
            {
                displayDay = START_DAY;
            }

            yield return null;
        }
        //dateText.text = $"SỐ NGÀY CÔNG TÁC: {START_DAY}";
        dateText.text = $"{START_DAY}";
       // continueButton.gameObject.SetActive(true);
    }

    // 1. Nút "Tiếp tục chơi"
    public void ContinueToNextScene()
    {
        // Quan trọng: Đảm bảo Time.timeScale được reset trước khi vào scene gameplay
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName); 
    }

    // 2. Nút "Reset Dữ Liệu"
    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        if (GalleryDataManager.Instance != null)
        {
            GalleryDataManager.Instance.ResetGalleryData();
        }
        
        Debug.Log("<color=orange>Tất cả dữ liệu game đã được reset!</color>");
        SceneManager.LoadScene(beginSceneName);
    }

    public void ToggleGalleryPanel()
    {
        if (GalleryPanel != null)
        {
            GalleryPanel.SetActive(!GalleryPanel.activeSelf);
        }
    }
}