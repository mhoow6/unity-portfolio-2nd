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

    List<GameObject> m_InventorySlots = new List<GameObject>();

    public override void OnClosed(){}

    public override void OnOpened()
    {
        // ���� �κ��丮�� ���缭 Content ����
        var userWeapons = GameManager.PlayerData.Inventory.Weapons;
        foreach (var weapon in userWeapons)
        {
            var _inst = Instantiate(WeaponUIPrefab, InventorySlotParent.content.transform);
            var inst = _inst.GetComponent<WeaponUI>();
            inst.SetWeaponData(weapon.Index, weapon.SlotIndex);

            m_InventorySlots.Add(inst.gameObject);
        }

        // ������ �κ��丮�� ���缭 Content ����
        var userItems = GameManager.PlayerData.Inventory.Items;
        foreach (var item in userItems)
        {
            var _inst = Instantiate(ItemUIPrefab, InventorySlotParent.content.transform);
            var inst = _inst.GetComponent<ItemUI>();
            inst.SetItemData(item.Index, item.Quantity, item.SlotIndex);

            m_InventorySlots.Add(inst.gameObject);
        }

        // ���� ��� ����

        // ���� ��� ����

        // ���� ���� �з� ��� ����

        // ó���� ���⸦ �����ϵ��� ��
    }
}
