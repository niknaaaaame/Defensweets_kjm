using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time = 360f;  
    public Text timerText;

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time < 0) time = 0;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        timerText.text = $"{minutes}:{seconds:00}";
    }
}
