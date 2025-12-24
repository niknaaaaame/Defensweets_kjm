using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionShow : MonoBehaviour
{
    [Header("Data")]
    public CollectionData[] entries;
    private int currentIndex = 0;

    [Header("UI")]
    public Text nameText;
    public Text descriptionText;
    public Image entryImage;

    void Start()
    {
        ShowEntry(0);
    }

    void ShowEntry(int index)
    {
        if (entries.Length == 0) return;

        currentIndex = Mathf.Clamp(index, 0, entries.Length - 1);
        var data = entries[currentIndex];

        nameText.text = data.displayName;
        descriptionText.text = data.description;
        entryImage.sprite = data.image;
    }

    public void NextPage()
    {
        ShowEntry(currentIndex + 1);
    }

    public void PrevPage()
    {
        ShowEntry(currentIndex - 1);
    }
}