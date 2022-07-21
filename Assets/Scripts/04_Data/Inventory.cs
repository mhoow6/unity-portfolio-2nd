using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using DatabaseSystem;

[Serializable]
public class Inventory : ISubscribable
{
    public delegate void InventoryDelegate(int index, int count);

    List<ItemData> m_Items = new List<ItemData>();
    [JsonIgnore] public InventoryDelegate OnItemAdd;
    [JsonIgnore] public InventoryDelegate OnWeaponAdd;
    [JsonIgnore] public InventoryDelegate OnItemRemove;
    [JsonIgnore] public InventoryDelegate OnWeaponRemove;

    public void AddItem(int itemIndex, int itemCount)
    {
        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);
        if (itemData.Index == 0)
        {
            Debug.LogWarning($"Index:{itemIndex}�� �������� �ƴ϶� �κ��丮�� �߰���Ű�� �ʾҽ��ϴ�");
            return;
        }

        var exist = m_Items.Find(item => item.Index == itemIndex);
        if (exist != null)
        {
            // �κ��丮�� �������� �ִ� ���
            exist.Quantity += itemCount;
        }
        else
        {
            // �κ��丮�� �������� �ƿ� ���� ���
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

    public void AddWeapon(int weaponIndex)
    {
        var itemData = TableManager.Instance.WeaponTable.Find(item => item.Index == weaponIndex);
        if (itemData.Index == 0)
        {
            Debug.LogWarning($"Index:{weaponIndex}�� ���Ⱑ �ƴ϶� �κ��丮�� �߰���Ű�� �ʾҽ��ϴ�");
            return;
        }

        m_Items.Add(new ItemData()
        {
            Index = weaponIndex,
            Quantity = 1
        });
    }

    public void RemoveWeapon(ItemData data)
    {
        if (m_Items.Contains(data))
        {
            m_Items.Remove(data);
        }
    }

    public ItemData FindItem(int itemIndex)
    {
        return m_Items.Find(item => item.Index == itemIndex);
    }

    /// <summary> �κ��丮�� �ִ� ��� �������� �ֿܼ� �����ݴϴ�. </summary>
    public void ShowInventoryToString()
    {
        int count = 1;
        foreach (var item in m_Items)
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
