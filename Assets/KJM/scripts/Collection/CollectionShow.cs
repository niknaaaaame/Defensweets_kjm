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
    public Image rangeImage;

    [Header("UI Roots")]
    public GameObject iconRoot;
    public GameObject entryDetailRoot;

    void Start()
    {
        ShowIconView();
    }

    void ShowIconView()
    {
        iconRoot.SetActive(true);
        entryDetailRoot.SetActive(false);
    }

    void ShowEntryView()
    {
        iconRoot.SetActive(false);
        entryDetailRoot.SetActive(true);
    }

    public void ShowEntry(int index)
    {
        if (entries.Length == 0) return;

        ShowEntryView(); 

        currentIndex = Mathf.Clamp(index, 0, entries.Length - 1);
        var data = entries[currentIndex];

        nameText.text = data.displayName;
        descriptionText.text = data.description;
        entryImage.sprite = data.image;
        rangeImage.sprite = data.rangeImage;
    }

    public void NextPage()
    {
        int nextIndex = currentIndex + 1;

        if (nextIndex >= entries.Length)
            nextIndex = 0;

        ShowEntry(nextIndex);
    }

    public void PrevPage()
    {
        int prevIndex = currentIndex - 1;

        if (prevIndex < 0)
            prevIndex = entries.Length - 1;

        ShowEntry(prevIndex);
    }

    public void BackToIconView()
    {
        ShowIconView();
    }
}