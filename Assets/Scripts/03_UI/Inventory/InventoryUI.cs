using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UI
{
    public override UIType Type => UIType.Inventory;

    public ScrollRect InventorySlotParent;

    public GameObject WeaponUIPrefab;
    public GameObject ItemUIPrefab;

    List<InventoryUIStoredData> m_InventorySlots = new List<InventoryUIStoredData>();

    public override void OnClosed(){}

    public override void OnOpened()
    {
        // 무기 인벤토리에 맞춰서 Content 생성
        var userWeapons = GameManager.PlayerData.Inventory.Weapons;
        foreach (var weapon in userWeapons)
        {
            var _inst = Instantiate(WeaponUIPrefab, InventorySlotParent.content.transform);
            var inst = _inst.GetComponent<WeaponUI>();
            inst.SetWeaponData(weapon.Index, weapon.SlotIndex);

            m_InventorySlots.Add(new InventoryUIStoredData()
            {
                SlotIdx = weapon.SlotIndex,
                GameObject = inst.gameObject
            });
        }

        // 아이템 인벤토리에 맞춰서 Content 생성
        var userItems = GameManager.PlayerData.Inventory.Items;
        foreach (var item in userItems)
        {
            var _inst = Instantiate(ItemUIPrefab, InventorySlotParent.content.transform);
            var inst = _inst.GetComponent<ItemUI>();
            inst.SetItemData(item.Index, item.Quantity, item.SlotIndex);

            m_InventorySlots.Add(new InventoryUIStoredData()
            {
                SlotIdx = item.SlotIndex,
                GameObject = inst.gameObject
            });
        }

        // 무기 토글 세팅

        // 소재 토글 세팅

        // 소재 내부 분류 토글 세팅

        // 처음엔 무기를 선택하도록 함
    }
}
