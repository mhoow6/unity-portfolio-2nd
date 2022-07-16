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
            // �κ��丮�� �������� �ִ� ���
            exist.Quantity += itemCount;
        }
        else
        {
            // �κ��丮�� �������� �ƿ� ���� ���
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

    /// <summary> �κ��丮�� �ִ� ��� �������� �ֿܼ� �����ݴϴ�. </summary>
    public void ShowInventoryToString()
    {
        foreach (var item in Items)
        {
            Debug.Log($"Index: {item.Index}, Quantity: {item.Quantity}");
        }
    }
}
