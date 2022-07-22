using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class GoldRefillItem : InventoryItem
{
    public override void Use(int itemIndex, int count)
    {
        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);

        GameManager.PlayerData.Gold += (itemData.Point * count);
    }
}
