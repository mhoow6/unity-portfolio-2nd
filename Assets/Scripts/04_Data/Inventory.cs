using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Inventory
{
    public List<ItemData> Items = new List<ItemData>();

    public void AddItem(int itemIndex, int itemCount)
    {
        var exist = Items.Find(item => item.Index == itemIndex);
        if (exist != null)
        {
            // 인벤토리에 아이템이 있는 경우
            exist.Quantity += itemCount;
        }
        else
        {
            // 인벤토리에 아이템이 아예 없는 경우
            Items.Add(new ItemData()
            {
                Index = itemIndex,
                Quantity = itemCount
            });
        }
    }

    public void RemoveItem(int itemIndex, int itemCount)
    {
        var exist = Items.Find(item => item.Index == itemIndex);
        if (exist != null)
        {
            exist.Quantity -= itemCount;
        }
    }

    /// <summary> 인벤토리에 있는 모든 아이템을 콘솔에 보여줍니다. </summary>
    public void ShowInventoryToString()
    {
        foreach (var item in Items)
        {
            Debug.Log($"Index: {item.Index}, Quantity: {item.Quantity}");
        }
    }
}
