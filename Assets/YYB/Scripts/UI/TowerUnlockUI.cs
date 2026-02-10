using UnityEngine;
using UnityEngine.UI;

public class TowerUnlockUI : MonoBehaviour
{
    [Header("Unlock Config")]
    [SerializeField] private string towerUnlockKey = "churros";

    [Header("Targets")]
    [SerializeField] private GameObject towerButtonObject;
    [SerializeField] private Button towerButton;

    private const string DefaultTowerButtonName = "TowerButton";

    private void Awake()
    {
        if (towerButtonObject == null)
            towerButtonObject = FindTowerButtonObject();

        if (towerButton == null && towerButtonObject != null)
            towerButton = towerButtonObject.GetComponent<Button>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe(Events.OnTowerUnlocked, OnTowerUnlocked);
        RefreshUnlockState();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(Events.OnTowerUnlocked, OnTowerUnlocked);
    }

    private void OnTowerUnlocked(object unlockedTowerKey)
    {
        if (unlockedTowerKey is string key && key != towerUnlockKey)
            return;

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

    private GameObject FindTowerButtonObject()
    {
        var child = transform.Find("TowerButton");
        if (child != null) return child.gameObject;

        return GetComponentInChildren<Button>()?.gameObject;
    }
}