using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Collection")]
public class CollectionData : ScriptableObject
{
    [Header("Image")]
    public Sprite image;

    [Header("Name")]
    public string displayName;

    [Header("Description")]
    [TextArea(5, 15)]
    public string description;

    [Header("Value")]
    public string[] ATK = new string[3];
    public string[] AS = new string[3];
    public string[] Range = new string[3];
    public string[] DrainRate = new string[3];
}