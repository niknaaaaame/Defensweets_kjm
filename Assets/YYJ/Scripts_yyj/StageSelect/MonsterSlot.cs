using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonsterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string monsterName;
    [TextArea] public string description;

    public MonsterTooltip tooltip;

    private Image myImage;

    void Start()
    {
        myImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Sprite spriteToSend = (myImage != null) ? myImage.sprite : null;
        tooltip.Show(monsterName, description, spriteToSend, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null) tooltip.Hide();
    }
}
