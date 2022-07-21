using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class Inventory : ISubscribable
{
    public delegate void InventoryDelegate(int itemIndex, int itemCount);

    List<ItemData> m_Items = new List<ItemData>();
    [JsonIgnore] public InventoryDelegate OnItemAdd;
    [JsonIgnore] public InventoryDelegate OnItemRemove;

    public void AddItem(int itemIndex, int itemCount)
    {
        var exist = m_Items.Find(item => item.Index == itemIndex);
        if (exist != null)
        {
            // 인벤토리에 아이템이 있는 경우
            exist.Quantity += itemCount;
        }
        else
        {
            // 인벤토리에 아이템이 아예 없는 경우
            m_Items.Add(new ItemData()
            {
                Index = itemIndex,
                Quantity = itemCount
            });
        }
        OnItemAdd?.Invoke(itemIndex, itemCount);
    }

    public void RemoveItem(int itemIndex, int itemCount)
    {
        var exist = m_Items.Find(item => item.Index == itemIndex);
        if (exist != null)
        {
            exist.Quantity -= itemCount;
            if (exist.Quantity <= 0)
            {
                m_Items.Remove(exist);
            }
            OnItemRemove?.Invoke(itemIndex, itemCount);
        }
    }

    public ItemData FindItem(int itemIndex)
    {
        return m_Items.Find(item => item.Index == itemIndex);
    }

    /// <summary> 인벤토리에 있는 모든 아이템을 콘솔에 보여줍니다. </summary>
    public void ShowInventoryToString()
    {
        int count = 1;
        foreach (var item in m_Items)
        {
            Debug.Log($"{count++}번째 아이템 슬롯: [Index: {item.Index}, Quantity: {item.Quantity}]");
        }
    }

    public void DisposeEvents()
    {
        OnItemAdd = null;
        OnItemRemove = null;
    }
}
