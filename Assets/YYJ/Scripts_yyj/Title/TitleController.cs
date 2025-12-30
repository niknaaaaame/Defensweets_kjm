using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    [SerializeField] private Text blinkText;
    [SerializeField] private Image blinkImage;
    [SerializeField] private float blinkSpeed = 3.0f;
    [SerializeField] private string nextSceneName = "StageSelectScene"; // 빌드 세팅에 맞게 이름 바꾸기

    // Update is called once per frame
    void Update()
    {
        HandleBlink();
        CheckInput();
    }

    void HandleBlink()
    {
        if (blinkText != null)
        {
            Color color1 = blinkText.color;
            Color color2 = blinkImage.color;
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1.0f) * 0.5f;
            color1.a = alpha;
            color2.a = alpha;
            blinkText.color = color1;
            blinkImage.color = color2;
        }
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
