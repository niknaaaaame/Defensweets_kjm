using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    [SerializeField] private string worldSceneName = "WorldSelectScene";

    public void GoToWorldSelect()
    {
        SceneManager.LoadScene(worldSceneName);
    }
}
