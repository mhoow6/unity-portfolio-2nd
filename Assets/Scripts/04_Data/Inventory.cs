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

    public IEnumerable<ReadOnlyItemSlotData> Items
    {
        get
        {
            List<ReadOnlyItemSlotData> result = new List<ReadOnlyItemSlotData>();
            foreach (var item in m_Items)
                result.Add(new ReadOnlyItemSlotData(item));

            return result;
        }
    }
    public IEnumerable<ReadOnlyWeaponSlotData> Weapons
    {
        get
        {
            List<ReadOnlyWeaponSlotData> result = new List<ReadOnlyWeaponSlotData>();
            foreach (var weapom in m_Weapons)
                result.Add(new ReadOnlyWeaponSlotData(weapom));

            return result;
        }
    }

    List<ItemSlotData> m_Items = new List<ItemSlotData>();
    List<WeaponSlotData> m_Weapons = new List<WeaponSlotData>();

    [JsonIgnore] public InventoryDelegate OnItemAdd;
    [JsonIgnore] public Action<int> OnWeaponAdd;
    [JsonIgnore] public InventoryDelegate OnItemRemove;
    [JsonIgnore] public Action<int> OnWeaponRemove;

    public void AddItem(int itemIndex, int itemCount)
    {
        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);
        if (itemData.Index == 0)
        {
            Debug.LogWarning($"Index:{itemIndex}는 아이템이 아니라서 인벤토리에 추가시키지 않았습니다");
            return;
        }

        var exist = m_Items.Find(item => item.Index == itemIndex);
        if (exist != null)
        {
            // 인벤토리에 아이템이 있는 경우
            exist.Quantity += itemCount;
        }
        else
        {
            // 인벤토리에 아이템이 아예 없는 경우
            m_Items.Add(new ItemSlotData()
            {
                Index = itemIndex,
                Quantity = itemCount,
                SlotIndex = m_Items.Count
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
            Debug.LogWarning($"Index:{weaponIndex}는 무기가 아니라서 인벤토리에 추가시키지 않았습니다");
            return;
        }

        m_Weapons.Add(new WeaponSlotData()
        {
            Index = weaponIndex,
            Level = 1,
            SlotIndex = m_Items.Count
        });
        OnWeaponAdd?.Invoke(weaponIndex);
    }

    public void RemoveWeapon(int weaponIndex, int slotIdx)
    {
        var exist = m_Weapons.Find(item => item.Index == weaponIndex && item.SlotIndex == slotIdx);
        if (exist != null)
        {
            m_Weapons.Remove(exist);
            OnWeaponRemove?.Invoke(weaponIndex);
        }
    }

    public ItemSlotData FindItem(int itemIndex)
    {
        return m_Items.Find(item => item.Index == itemIndex);
    }

    public WeaponSlotData FindWeapon(int weaponIndex, int slotIdx)
    {
        return m_Weapons.Find(item => item.Index == weaponIndex && item.SlotIndex == slotIdx);
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
