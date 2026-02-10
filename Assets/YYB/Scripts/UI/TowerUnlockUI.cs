using UnityEngine;
using UnityEngine.UI;

public class TowerUnlockUI : MonoBehaviour
{
    [Header("Unlock Config")]
    [SerializeField] private string towerUnlockKey = "churros";

    [Header("Targets")]
    [SerializeField] private GameObject towerButtonObject;
    [SerializeField] private Button towerButton;

    private void Awake()
    {
        if (towerButtonObject == null)
            towerButtonObject = gameObject;

        if (towerButton == null)
            towerButton = towerButtonObject.GetComponent<Button>();
    }

    private void OnEnable()
    {
        RefreshUnlockState();
    }

    public void RefreshUnlockState()
    {
        bool isUnlocked = ProgressionSave.IsTowerUnlocked(towerUnlockKey);

        if (towerButtonObject != null)
            towerButtonObject.SetActive(isUnlocked);

        if (towerButton != null)
            towerButton.interactable = isUnlocked;
    }
}