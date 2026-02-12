using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel Instance { get; private set; }

    public GameObject towerInfoPanel;

    [SerializeField] TextMeshProUGUI towerNameText;
    [SerializeField] TextMeshProUGUI towerInfoText;
    [SerializeField] TextMeshProUGUI towerSumText;
    [SerializeField] Button upgradeButton;
    [SerializeField] Button destroyButton;
    [SerializeField] Image previewImage;
    [SerializeField] AudioSource upgradeAudio;
    [SerializeField] AudioSource destroyAudio;

    [SerializeField] private float hideDelay = 2f;

    private Transform currentTower = null;
    private TowerSO towerData = null;
    private TowerInterface towerInterface = null;
    private Coroutine delayCoroutine;

    private void Awake()
    {
        Instance = this;
        towerInfoPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowTowerInfo(GameObject tower, int level)
    {
        currentTower = tower.transform;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTower.position);
        towerInfoPanel.transform.position = screenPos + new Vector3(115, 0, 0);

        towerInterface = tower.GetComponent<TowerInterface>();
        towerData = towerInterface.GetTowerData();

        if (level < 2)
        {
            towerNameText.text = $"{towerData.towerName}";
            towerSumText.text = $"{towerData.summary}";
            towerInfoText.text = $"- level {level + 1} -\n공격력: {towerData.levels[level].damage}   공격속도: {towerData.levels[level].attackSpeed}\n-------------------\n업그레이드 비용\nsugar: {towerData.levels[level].upgradeCostSugar}    crystal: {towerData.levels[level].specialCostCrystal}";
        }
        else
        {
            towerNameText.text = $"{towerData.towerName}";
            towerSumText.text = $"{towerData.summary}";
            towerInfoText.text = $"- level {level + 1} -\n공격력: {towerData.levels[level].damage}   공격속도: {towerData.levels[level].attackSpeed}\n-------------------\n      최대레벨";
        }

        previewImage.sprite = towerData.levels[level].preview;

        towerInfoPanel.SetActive(true);

        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
        }

        delayCoroutine = StartCoroutine(DelayHidePanel(hideDelay));
    }

    IEnumerator DelayHidePanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        towerInfoPanel.SetActive(false);
        delayCoroutine = null;

    }

    public void ToggleTowerInfo(GameObject tower, int curLevel)
    {
        if (towerInfoPanel.activeSelf && currentTower == tower.transform)
        {
            towerInfoPanel.SetActive(false);
        }
        else
        {
            ShowTowerInfo(tower, curLevel);
        }
    }

    public void OnCilckUpgrade()
    {
        if (towerInterface.Upgrade())
        {
            upgradeAudio.Play();
        }
    }

    public void OnClickDestroy()
    {
        towerInterface.Destroy();
        destroyAudio.Play();

        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
        }
        delayCoroutine = StartCoroutine(DelayHideAfterSound(destroyAudio.clip.length));
    }

    IEnumerator DelayHideAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        towerInfoPanel.SetActive(false);
        delayCoroutine = null;
    }
}
