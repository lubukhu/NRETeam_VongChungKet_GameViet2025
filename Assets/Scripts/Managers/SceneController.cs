using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Obvious.Soap;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingSceneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button continueButton;
    [SerializeField] private IntVariable endDay;
    [SerializeField] private float rewindSpeed = 0.01f; // Tốc độ tua ngược

    private const int START_DAY = 1;

    void Start()
    {
        continueButton.gameObject.SetActive(false);
        StartCoroutine(RewindDateRoutine());
    }

    private IEnumerator RewindDateRoutine()
    {
        int displayDay = endDay.Value;

        // Tua ngược từ ngày kết thúc về ngày bắt đầu
        while (displayDay > START_DAY)
        {
            // Chuyển đổi tổng số ngày thành định dạng Ngày/Tháng/Năm
            dateText.text = ConvertDaysToDateString(displayDay);

            // Giảm dần số ngày, tốc độ có thể điều chỉnh
            displayDay = Mathf.Max(START_DAY, displayDay - (int)(1 / rewindSpeed * Time.deltaTime));

            yield return null; // Chờ đến frame tiếp theo
        }

        // Hiển thị ngày cuối cùng và kích hoạt nút
        dateText.text = ConvertDaysToDateString(START_DAY);
        continueButton.gameObject.SetActive(true);
    }

    // Hàm chuyển đổi từ tổng số ngày sang định dạng dd/mm/yyyy
    private string ConvertDaysToDateString(int totalDays)
    {
        int year = 2050 + (totalDays - 1) / 365;
        int dayOfYear = (totalDays - 1) % 365;
        int month = 1;
        int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        while(dayOfYear >= daysInMonth[month-1])
        {
            dayOfYear -= daysInMonth[month-1];
            month++;
        }
        int day = dayOfYear + 1;

        return $"{day:D2}/{month:D2}/{year}";
    }

    public void ContinueToNextScene()
    {
        // Thêm logic chuyển về scene mở đầu hoặc scene gameplay mới ở đây
        SceneManager.LoadScene("Scene_Quang"); 
    }
}