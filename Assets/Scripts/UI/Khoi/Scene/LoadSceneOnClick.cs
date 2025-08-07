using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour

{
    public string sceneName;

    public void ContinueToNextScene()
    {
        // Quan trọng: Đảm bảo Time.timeScale được reset trước khi vào scene gameplay
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName); 
    }

}