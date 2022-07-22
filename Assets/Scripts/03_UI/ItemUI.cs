using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using System;
using UnityEngine.EventSystems;

public class ItemUI : Display
{
    public Image ItemFrame;
    public Image ItemIcon;
    public Image RarityBackground;

    // 2 3 4 5 6
    public List<GameObject> Stars = new List<GameObject>();
    public Text ItemCount;

    protected int m_SlotIdx;

    public void SetItemData(int itemIndex, int itemCount)
    {
        if (itemIndex < 5000 && itemIndex > 6000)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);

        // 아이콘
        ItemIcon.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{itemData.IconName}");
        ItemCount.text = $"x<size=24>{itemCount}</size>";

        // 별의 갯수
        Stars.ForEach(star => star.gameObject.SetActive(false));
        Stars[itemData.StarCount - 2].gameObject.SetActive(true);

        // 배경 색상
        RarityBackground.color = GameManager.GlobalData.RarityBackgroundColors[itemData.StarCount - 2];
    }

    public void SetItemData(int itemIndex, int itemCount, int slotIdx)
    {
        SetItemData(itemIndex, itemCount);

        m_SlotIdx = slotIdx;
    }
}
