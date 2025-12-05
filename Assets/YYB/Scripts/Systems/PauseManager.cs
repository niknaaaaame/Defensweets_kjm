using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject settingsPanel; 

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;
    }

    public bool IsPaused => isPaused;

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }
    
    public void ToggleSettings()
    {
        if (settingsPanel == null)
        {
            TogglePause();
            return;
        }

        bool show = !settingsPanel.activeSelf;
        settingsPanel.SetActive(show);

        if (show) Pause();
        else Resume();
    }
}
