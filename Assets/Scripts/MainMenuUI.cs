using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // Bắt đầu game với scene mặc định (nếu cần)
    public void StartGame()
    {
        SceneManager.LoadScene("ChooseMap"); // Hoặc đổi tên scene nếu muốn
    }

    // Chọn map sa mạc
    public void OnDesertMapClicked()
    {
        SceneManager.LoadScene("Forest"); // Map sa mạc → scene Forest
    }

    // Chọn map đêm tím
    public void OnNightMapClicked()
    {
        SceneManager.LoadScene("Night"); // Map đêm tím → scene Night
    }

    // Chọn map tuyết trắng
    public void OnSnowMapClicked()
    {
        SceneManager.LoadScene("Snow"); // Map tuyết trắng → scene Snow
    }

    // Thoát game
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Test khi chạy trong Editor
#endif
    }
}