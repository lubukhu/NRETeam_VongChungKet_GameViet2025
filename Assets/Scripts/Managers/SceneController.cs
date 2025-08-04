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
    [SerializeField] private GameObject settingsPanel; // Tham chiếu đến Panel Settings

    [Header("Data Inputs")]
    [SerializeField] private IntVariable endDay;
    [SerializeField] private string gameplaySceneName = "Scene_Quang"; 
    
    [Header("Config")]
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private float rewindSpeed = 0.007f;

    private const int START_DAY = 1;

    void Start()
    {
        // Vô hiệu hóa các UI không cần thiết lúc bắt đầu
       // continueButton.gameObject.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // Hiển thị ngày kết thúc
        dateText.text = $"SỐ NGÀY CÔNG TÁC: {endDay.Value}";

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
            dateText.text = $"SỐ NGÀY CÔNG TÁC: {displayDay}";
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

        dateText.text = $"SỐ NGÀY CÔNG TÁC: {START_DAY}";
       // continueButton.gameObject.SetActive(true);
    }

    // --- CÁC HÀM MỚI CHO CÁC NÚT BẤM ---

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
        // Có thể thêm hiệu ứng hình ảnh hoặc âm thanh để xác nhận
        Debug.Log("<color=orange>Tất cả dữ liệu PlayerPrefs đã được xóa!</color>");
        // Sau khi xóa, có thể load lại scene mở đầu để bắt đầu lại hoàn toàn
         SceneManager.LoadScene("Scene_Khoi");
    }

    // 3. Nút "Setting"
    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            // Bật/tắt Panel Settings
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        Debug.Log("Chức năng Setting sẽ được phát triển sau.");
    }

    // 4. Nút "Thoát"
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