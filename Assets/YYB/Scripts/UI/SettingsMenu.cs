using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    // 현재 스테이지 다시 시작
    public void OnClickRestart()
    {
        // 혹시 일시정지 상태였다면 먼저 풀기
        Time.timeScale = 1f;

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    // 스테이지 선택 씬으로 이동
    public void OnClickStageSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelectScene");
    }

    // (선택) 설정 닫기 버튼용
    public void OnClickClose()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ToggleSettings();
        }
        else
        {
            // 혹시 PauseManager 안 쓰는 씬이면 그냥 패널만 끄기
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void OnClickCloseSound()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.BottonSettings();
        }
        else
        {
            // 혹시 PauseManager 안 쓰는 씬이면 그냥 패널만 끄기
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
