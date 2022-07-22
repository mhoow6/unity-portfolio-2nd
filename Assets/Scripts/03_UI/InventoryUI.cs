using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UI, IGameEventListener
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
        GameEventSystem.RemoveListener(this);

        foreach (var slot in m_InventorySlots)
            Destroy(slot.GameObject);
        m_InventorySlots.Clear();

        WeaponToggle.onValueChanged.RemoveAllListeners();
        ItemToggle.onValueChanged.RemoveAllListeners();
    }

    public override void OnOpened()
    {
        GameEventSystem.AddListener(this);

        // ���� �κ��丮�� ���缭 Content ����
        var userWeapons = GameManager.PlayerData.Inventory.Weapons;
        foreach (var weapon in userWeapons)
        {
            var _inst = Instantiate(WeaponUIPrefab, InventorySlotParent.content.transform);
            var inst = _inst.GetComponent<WeaponUI>();
            inst.SetWeaponData(weapon.Index, weapon.SlotIndex);

            m_InventorySlots.Add(new InventoryUIStoredData()
            {
                ItemIndex = weapon.Index,
                SlotIdx = weapon.SlotIndex,
                GameObject = inst.gameObject
            });
        }

        // ������ �κ��丮�� ���缭 Content ����
        var userItems = GameManager.PlayerData.Inventory.Items;
        foreach (var item in userItems)
        {
            var _inst = Instantiate(ItemUIPrefab, InventorySlotParent.content.transform);
            var inst = _inst.GetComponent<ItemUI>();
            inst.SetItemData(item.Index, item.Quantity, item.SlotIndex);

            var gButton = inst.gameObject.AddComponent<GenericButton>();
            gButton.onButtonClick.AddListener(() =>
            {
                var explainUI = GameManager.UISystem.OpenWindow<ItemExplainUI>(UIType.ItemExplain, false);
                explainUI.SetData(item.Index);
            });

            m_InventorySlots.Add(new InventoryUIStoredData()
            {
                ItemIndex = item.Index,
                SlotIdx = item.SlotIndex,
                GameObject = inst.gameObject
            });
        }

        // ���� ��� ����
        WeaponToggle.onValueChanged.AddListener(delegate
        {
            WeaponToggleOnValueChanged(WeaponToggle);
        });

        // ���� ��� ����
        ItemToggle.onValueChanged.AddListener(delegate
        {
            ItemToggleOnValueChanged(ItemToggle);
        });

        // ���� - �Ҹ�ǰ ��� ����
        ItemCatergoy1Toggle.onValueChanged.AddListener(delegate
        {
            ItemCategory1ToggleOnValueChanged(ItemCatergoy1Toggle);
        });

        // ���� - ������� ��� ����
        ItemCatergoy2Toggle.onValueChanged.AddListener(delegate
        {
            ItemCategory2ToggleOnValueChanged(ItemCatergoy2Toggle);
        });

        // ���� - �ռ����� ��� ����
        ItemCatergoy3Toggle.onValueChanged.AddListener(delegate
        {
            ItemCategory3ToggleOnValueChanged(ItemCatergoy3Toggle);
        });

        // ���� - �̺�Ʈ ��� ����
        ItemCatergoy4Toggle.onValueChanged.AddListener(delegate
        {
            ItemCategory4ToggleOnValueChanged(ItemCatergoy4Toggle);
        });

        // ó�� �������� ������ ȭ�� �����ϱ�
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
            ItemCatergoy1Toggle.gameObject.SetActive(true);
            ItemCatergoy2Toggle.gameObject.SetActive(true);
            ItemCatergoy3Toggle.gameObject.SetActive(true);
            ItemCatergoy4Toggle.gameObject.SetActive(true);

            WeaponToggle.GetComponent<Image>().color = ToggleColors[0];
            ItemToggle.GetComponent<Image>().color = ToggleColors[1];

            ItemCatergoy1Toggle.isOn = true;
            ItemCatergoy1Toggle.onValueChanged?.Invoke(true);
        }
    }

    void ItemCategory1ToggleOnValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            m_InventorySlots.ForEach(slot => slot.GameObject.SetActive(false));
            ItemCatergoy1Toggle.GetComponent<Image>().color = InnerToggleColors[1];
            ItemCatergoy2Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy3Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy4Toggle.GetComponent<Image>().color = InnerToggleColors[0];

            var group = TableManager.Instance.ItemTable.FindAll(item => item.GroupType == ItemGroupType.Consume);
            var userGroupItems = m_InventorySlots.Where(slot => group.Find(item => item.Index == slot.ItemIndex).Index != 0);
            foreach (var item in userGroupItems)
                item.GameObject.SetActive(true);
        }
    }

    void ItemCategory2ToggleOnValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            m_InventorySlots.ForEach(slot => slot.GameObject.SetActive(false));
            ItemCatergoy1Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy2Toggle.GetComponent<Image>().color = InnerToggleColors[1];
            ItemCatergoy3Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy4Toggle.GetComponent<Image>().color = InnerToggleColors[0];

            var groups = TableManager.Instance.ItemTable.FindAll(item => item.GroupType == ItemGroupType.BasicMaterial);
            var userGroupItems = m_InventorySlots.Where(slot => groups.Find(item => item.Index == slot.ItemIndex).Index != 0);
            foreach (var item in userGroupItems)
                item.GameObject.SetActive(true);
        }
    }

    void ItemCategory3ToggleOnValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            m_InventorySlots.ForEach(slot => slot.GameObject.SetActive(false));
            ItemCatergoy1Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy2Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy3Toggle.GetComponent<Image>().color = InnerToggleColors[1];
            ItemCatergoy4Toggle.GetComponent<Image>().color = InnerToggleColors[0];

            var groups = TableManager.Instance.ItemTable.FindAll(item => item.GroupType == ItemGroupType.SyntheticMaterial);
            var userGroupItems = m_InventorySlots.Where(slot => groups.Find(item => item.Index == slot.ItemIndex).Index != 0);
            foreach (var item in userGroupItems)
                item.GameObject.SetActive(true);
        }
    }

    void ItemCategory4ToggleOnValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            m_InventorySlots.ForEach(slot => slot.GameObject.SetActive(false));
            ItemCatergoy1Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy2Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy3Toggle.GetComponent<Image>().color = InnerToggleColors[0];
            ItemCatergoy4Toggle.GetComponent<Image>().color = InnerToggleColors[1];

            var groups = TableManager.Instance.ItemTable.FindAll(item => item.GroupType == ItemGroupType.Event);
            var userGroupItems = m_InventorySlots.Where(slot => groups.Find(item => item.Index == slot.ItemIndex).Index != 0);
            foreach (var item in userGroupItems)
                item.GameObject.SetActive(true);
        }
    }

    public void Listen(GameEvent gameEvent)
    {
        
    }

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        switch (gameEvent)
        {
            case GameEvent.LOBBY_DestroyItem:
                {
                    int itemIndex = (int)args[0];

                    var slot = m_InventorySlots.Find(slot => slot.ItemIndex == itemIndex);
                    Destroy(slot.GameObject);
                    m_InventorySlots.Remove(slot);

                    break;
                }
                
        }
    }
}
