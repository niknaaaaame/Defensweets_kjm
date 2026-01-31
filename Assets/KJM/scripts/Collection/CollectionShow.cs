using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CollectionShow : MonoBehaviour
{
    [Header("Data")]
    public CollectionData[] entries;
    private int currentIndex = 0;

    [Header("Entry")]
    public Text descriptionText;
    public Text ATKText;
    public Text ASText;
    public Text rangeText;
    public Text drainRateText;
    public Image entryImage;

    [Header("UI Roots")]
    public GameObject iconRoot;
    public GameObject entryDetailRoot;

    [Header("Icon Images")]
    public Image[] iconImages;

    [Header("Level Toggles")]
    public Toggle[] levelToggles;

    public int unlockCount = 3;

    private int currentLevel;

    void Start()
    {
        ShowIconView();
    }

    void Update()
    {
        if (entryDetailRoot.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentLevel = 0;
                PrevPage();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentLevel = 0;
                NextPage();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToIconView();
            }
        }
        else if (iconRoot.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("StageSelectScene");
            }
        }
    }

    void ShowIconView()
    {
        iconRoot.SetActive(true);
        entryDetailRoot.SetActive(false);

        for (int i = 0; i < iconImages.Length; i++)
        {
            bool unlocked = i < unlockCount;

            if (unlocked)
            {
                iconImages[i].color = Color.white;
            }
            else
            {
                iconImages[i].color = new Color32(125, 125, 125, 255);
            }
        }

    }
    void ShowEntryView()
    {
        currentLevel = 0;

        iconRoot.SetActive(false);
        entryDetailRoot.SetActive(true);

        RefreshLevelToggleUI();
    }

    public void ShowEntry(int index)
    {
        if (index >= unlockCount) return;
        if (entries.Length == 0) return;

        ShowEntryView();

        currentIndex = Mathf.Clamp(index, 0, entries.Length - 1);

        RefreshEntryUI();
    }
    void RefreshEntryUI()
    {
        var data = entries[currentIndex];

        descriptionText.text = data.description;
        entryImage.sprite = data.image;
        ATKText.text = data.ATK[currentLevel];
        ASText.text = data.AS[currentLevel];
        rangeText.text = data.Range[currentLevel];
        drainRateText.text = data.DrainRate[currentLevel];
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
    public void SetLevel(int level)
    {
        currentLevel = level;

        RefreshEntryUI();
        RefreshLevelToggleUI();
    }
    void RefreshLevelToggleUI()
    {
        for (int i = 0; i < levelToggles.Length; i++)
        {
            levelToggles[i].SetIsOnWithoutNotify(i == currentLevel);
        }
    }
}