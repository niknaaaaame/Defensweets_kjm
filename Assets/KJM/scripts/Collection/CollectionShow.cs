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
    public GameObject lockImage;
    public GameObject[] lockedImages;

    [Header("UI Roots")]
    public GameObject iconRoot;
    public GameObject entryDetailRoot;

    public int unlockCount = 3;

    void Start()
    {
        ShowIconView();
    }

    void ShowIconView()
    {
        iconRoot.SetActive(true);
        entryDetailRoot.SetActive(false);

        for (int i = 0; i < lockedImages.Length; i++)
        {
            bool isLocked = i >= unlockCount;
            lockedImages[i].SetActive(isLocked);
        }
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

        bool unlocked = currentIndex < unlockCount;

        if (unlocked)
        {
            nameText.text = data.displayName;
            descriptionText.text = data.description;
            entryImage.sprite = data.image;
            rangeImage.sprite = data.rangeImage;

            lockImage.SetActive(false);
        }
        else
        {
            nameText.text = "미해금";
            descriptionText.text = "미해금";
            entryImage.sprite = null;    
            rangeImage.sprite = null;

            lockImage.SetActive(true);
        }
    }

    public void NextPage()
    {
        if (unlockCount <= 0) return;

        int nextIndex = currentIndex + 1;

        if (nextIndex >= unlockCount)
            nextIndex = 0;

        ShowEntry(nextIndex);
    }

    public void PrevPage()
    {
        if (unlockCount <= 0) return;

        int prevIndex = currentIndex - 1;

        if (prevIndex < 0)
            prevIndex = unlockCount - 1;

        ShowEntry(prevIndex);
    }

    public void BackToIconView()
    {
        ShowIconView();
    }
}