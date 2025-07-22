using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public PauseMenuUI pauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
                pauseMenu.Pause();
            else
                pauseMenu.Resume();
        }
    }
} 