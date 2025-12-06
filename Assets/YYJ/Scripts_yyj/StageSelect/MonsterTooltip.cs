using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterTooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPanel;

    [SerializeField] private Image monsterImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descText;

    [SerializeField] private Vector3 offset = new Vector3(2f, 2f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    public void Show(string name, string desc, Sprite sprite, Vector3 position)
    {
        tooltipPanel.SetActive(true);
        nameText.text = name;
        descText.text = desc;
        if (monsterImage != null && sprite != null)
        {
            monsterImage.sprite = sprite;
            monsterImage.gameObject.SetActive(true);
        }
        else if (monsterImage != null)
        {
            monsterImage.gameObject.SetActive(false);
        }
        // 위치 설정
        transform.position = position + offset;
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
