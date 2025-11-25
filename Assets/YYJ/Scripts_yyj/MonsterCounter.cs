using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCounter : MonoBehaviour     // 그냥 태그로 읽는 방식
{
    [SerializeField] private Text countText;
    [SerializeField] private string pretext = "Monsters : ";
    [SerializeField] private float refreshRate = 0.1f;

    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= refreshRate)
        {
            CountAndDisplay();
            timer = 0f;
        }
    }

    void CountAndDisplay()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        int count = monsters.Length;

        if (countText != null)
        {
            countText.text = $"{pretext}{count}";
        }
    }
}
