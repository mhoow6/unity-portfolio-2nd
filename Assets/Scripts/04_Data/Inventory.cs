using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class Inventory : ISubscribable
{
    public delegate void InventoryDelegate(int itemIndex, int itemCount);

    public List<ItemData> Items = new List<ItemData>();
    [JsonIgnore] public InventoryDelegate OnItemAdd;
    [JsonIgnore] public InventoryDelegate OnItemRemove;

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
        OnItemAdd?.Invoke(itemIndex, itemCount);
    }

    public void RemoveItem(int itemIndex, int itemCount)
    {
        var exist = Items.Find(item => item.Index == itemIndex);
        if (exist != null)
        {
            exist.Quantity -= itemCount;
            if (exist.Quantity <= 0)
            {
                Items.Remove(exist);
            }
            OnItemRemove?.Invoke(itemIndex, itemCount);
        }
    }

    /// <summary> �κ��丮�� �ִ� ��� �������� �ֿܼ� �����ݴϴ�. </summary>
    public void ShowInventoryToString()
    {
        int count = 1;
        foreach (var item in Items)
        {
            Debug.Log($"{count++}��° ������ ����: [Index: {item.Index}, Quantity: {item.Quantity}]");
        }
    }

    public void DisposeEvents()
    {
        OnItemAdd = null;
        OnItemRemove = null;
    }
}
