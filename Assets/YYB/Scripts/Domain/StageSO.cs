using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Stage")]
public class StageSO : ScriptableObject
{
    public string stageId;
    public int initialSugar;
    public int initialCrystal;
    public int gateHp;
    public WaveSO[] waves;
}
