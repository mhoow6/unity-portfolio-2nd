using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class ItemUI : Display
{
    public Image ItemIcon;
    public Image RarityBackground;

    // 2 3 4 5 6
    public List<GameObject> Stars = new List<GameObject>();
    public Text ItemCount;

    public List<Color> RarityBackgroundColors = new List<Color>(5);

    public void SetData(int itemIndex, int itemCount)
    {
        if (itemIndex < 5000)
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
        RarityBackground.color = RarityBackgroundColors[itemData.StarCount - 2];
    }
}
