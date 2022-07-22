using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyRefillItem : InventoryItem
{
    public override void Use(int itemIndex, int count)
    {
        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);

        GameManager.PlayerData.Energy += (itemData.Point * count);
    }
}
