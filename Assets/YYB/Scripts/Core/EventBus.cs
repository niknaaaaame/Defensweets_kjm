using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventBus
{
    private static readonly Dictionary<string, Action<object>> table = new();

    public static void Subscribe(string key, Action<object> callback)
    {
        if (!table.ContainsKey(key)) table[key] = delegate { };
        table[key] += callback;
    }

    public static void Unsubscribe(string key, Action<object> callback)
    {
        if (table.ContainsKey(key)) table[key] -= callback;
    }

    public static void Publish(string key, object arg = null)
    {
        if (table.ContainsKey(key)) table[key].Invoke(arg);
    }
}

// 공용 키를 상수로 관리
public static class Events
{
    public const string OnMonsterKilled = "OnMonsterKilled";   // arg: int rewardSugar
    public const string OnWaveCleared = "OnWaveCleared";      // arg: int waveIndex
    public const string OnStateChanged = "OnStateChanged";     // arg: GameState
    public const string OnResourcePing = "OnResourcePing";     // UI 갱신 등
}
