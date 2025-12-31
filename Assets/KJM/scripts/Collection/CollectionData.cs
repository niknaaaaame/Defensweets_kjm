using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Collection")]
public class CollectionData : ScriptableObject
{
    [Header("Image")]
    public Sprite image;
    public Sprite rangeImage;

    [Header("Name")]
    public string displayName;

    [Header("Description")]
    [TextArea(5, 15)]
    public string description;
}