using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConnector : MonoBehaviour
{
    [Header("컬렉션 씬 이름")]
    public string collectionSceneName = "CollectionScene";

   public void GoToCollection() // 컬렉션 씬 으로 이동
   {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("PrevScene", currentScene);
        PlayerPrefs.Save();

        Debug.Log("현재 씬 저장됨. 컬렉션으로 이동");

        SceneManager.LoadScene(collectionSceneName);
   }
   // 이전 씬으로 복귀
   public void ReturnToPrevious()
   {
        string prevScene = PlayerPrefs.GetString("PrevScene", "WorldSelectScene");  // 저장된 씬 이름 가저옴 (없다면 월들 선택 씬으로)

        Debug.Log("이전 씬으로 이동");

        SceneManager.LoadScene(prevScene);
    }
}
