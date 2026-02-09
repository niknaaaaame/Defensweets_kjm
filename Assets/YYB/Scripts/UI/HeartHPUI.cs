using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHPUI : MonoBehaviour
{
    public static HeartHPUI Instance { get; private set; }

    [Header("하트 이미지들 (왼쪽→오른쪽 순서로)")]
    [SerializeField] private List<Image> heartImages = new List<Image>();

    [SerializeField] private int maxHP = 5;

    private int currentHP;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 에디터에서 하트 5개 넣고, maxHP 안 맞으면 자동 맞추기
        if (heartImages.Count > 0)
        {
            maxHP = heartImages.Count;
        }
    }

    public void Init(int hp)
    {
        maxHP = Mathf.Max(0, hp);
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateHearts();
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            // i < currentHP 면 켬, 아니면 끔
            // Heart0,1,2,3,4 로 세팅했기 때문에 오른쪽부터 꺼진다.
            bool active = i < currentHP;
            if (heartImages[i] != null)
                heartImages[i].enabled = active;
        }
    }
}
