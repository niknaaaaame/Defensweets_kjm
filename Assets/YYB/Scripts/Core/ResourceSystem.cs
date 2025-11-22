using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    public static ResourceSystem Instance { get; private set; }

    [Header("Current")]
    [SerializeField] private int sugar;
    [SerializeField] private int crystal;

    [SerializeField] TextMeshProUGUI sugarText;
    [SerializeField] TextMeshProUGUI crystalText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        EventBus.Subscribe(Events.OnMonsterKilled, OnMonsterKilledObj);
    }

    void Update()
    {
        sugarText.text = $"Sugar: {Sugar}";
        crystalText.text = $"Crystal: {Crystal}";
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(Events.OnMonsterKilled, OnMonsterKilledObj);
    }

    private void OnMonsterKilledObj(object obj)
    {
        if (obj is int sugar) AddSugar(sugar);
        EventBus.Publish(Events.OnResourcePing, null);
    }

    // 초기화 (스테이지 시작 시 GameManager가 호출)
    public void Setup(int startSugar, int startCrystal)
    {
        sugar = startSugar;
        crystal = startCrystal;
        EventBus.Publish(Events.OnResourcePing, null);
    }

    public bool TryUseSugar(int amount)
    {
        if (sugar < amount) return false;
        sugar -= amount;
        EventBus.Publish(Events.OnResourcePing, null);
        return true;
    }

    public bool TryUseCrystal(int amount)
    {
        if (crystal < amount) return false;
        crystal -= amount;
        EventBus.Publish(Events.OnResourcePing, null);
        return true;
    }

    public void AddSugar(int amount)
    {
        sugar += amount;
        EventBus.Publish(Events.OnResourcePing, null);
    }

    public void AddCrystal(int amount)
    {
        crystal += amount;
        EventBus.Publish(Events.OnResourcePing, null);
    }

    public int Sugar => sugar;
    public int Crystal => crystal;
}