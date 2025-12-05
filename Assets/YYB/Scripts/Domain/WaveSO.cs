using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Wave")]
public class WaveSO : ScriptableObject
{
    public float prepareTime;      
    public SpawnGroup[] spawns;    
    public Reward reward;          
}

[System.Serializable]
public struct SpawnGroup
{
    //public MonsterSO monster;
    public MonsterSO_YYJ monster;
    public int count;
    public float interval;
    public float startDelay;
}

[System.Serializable]
public struct Reward
{
    public int sugar;          
    public int crystalBonus;   
}
