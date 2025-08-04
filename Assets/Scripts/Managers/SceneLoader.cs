using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Hàm này là public để EventListener có thể thấy và gọi nó.
    // Nó nhận vào một tham số là tên của scene cần tải.
    public void LoadSceneByName(string sceneName)
    {
        // Kiểm tra để chắc chắn tên scene không bị bỏ trống
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene Name cannot be empty!", this.gameObject);
        }
    }
}