using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UI
{
    public override UIType Type => UIType.Inventory;

    public ScrollRect InventorySlotParent;

    public GameObject WeaponUIPrefab;
    public GameObject ItemUIPrefab;

    public Toggle WeaponToggle;
    public Toggle ItemToggle;
    public Toggle ItemCatergoy1Toggle;
    public Toggle ItemCatergoy2Toggle;
    public Toggle ItemCatergoy3Toggle;
    public Toggle ItemCatergoy4Toggle;

    // 0: Deselect 1: Select
    public List<Color> ToggleColors = new List<Color>();
    public List<Color> InnerToggleColors = new List<Color>();
         
    List<InventoryUIStoredData> m_InventorySlots = new List<InventoryUIStoredData>();

    public override void OnClosed()
    {
        foreach (var slot in m_InventorySlots)
            Destroy(slot.GameObject);
        m_InventorySlots.Clear();

        WeaponToggle.onValueChanged.RemoveAllListeners();
        ItemToggle.onValueChanged.RemoveAllListeners();
    }

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
        WeaponToggle.onValueChanged.AddListener(delegate
        {
            WeaponToggleOnValueChanged(WeaponToggle);
        });

        // 소재 토글 세팅
        ItemToggle.onValueChanged.AddListener(delegate
        {
            ItemToggleOnValueChanged(ItemToggle);
        });

        // 처음 열렸을때 보여줄 화면 구성하기
        WeaponToggle.isOn = true;
        WeaponToggle.onValueChanged?.Invoke(true);

    }

    void WeaponToggleOnValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            m_InventorySlots.ForEach(slot => slot.GameObject.SetActive(false));
            ItemCatergoy1Toggle.gameObject.SetActive(false);
            ItemCatergoy2Toggle.gameObject.SetActive(false);
            ItemCatergoy3Toggle.gameObject.SetActive(false);
            ItemCatergoy4Toggle.gameObject.SetActive(false);

            WeaponToggle.GetComponent<Image>().color = ToggleColors[1];
            ItemToggle.GetComponent<Image>().color = ToggleColors[0];

            var weapons = m_InventorySlots.Where(slot => GameManager.PlayerData.Inventory.FindWeaponBySlotIndex(slot.SlotIdx) != null);
            foreach (var weapon in weapons)
                weapon.GameObject.SetActive(true);
        }
    }

    void ItemToggleOnValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            m_InventorySlots.ForEach(slot => slot.GameObject.SetActive(false));
            ItemCatergoy1Toggle.gameObject.SetActive(true);
            ItemCatergoy2Toggle.gameObject.SetActive(true);
            ItemCatergoy3Toggle.gameObject.SetActive(true);
            ItemCatergoy4Toggle.gameObject.SetActive(true);

            WeaponToggle.GetComponent<Image>().color = ToggleColors[0];
            ItemToggle.GetComponent<Image>().color = ToggleColors[1];

            var items = m_InventorySlots.Where(slot => GameManager.PlayerData.Inventory.FindItemBySlotIndex(slot.SlotIdx) != null);
            foreach (var item in items)
                item.GameObject.SetActive(true);
        }
    }

}
