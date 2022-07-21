using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using System.Linq;

public class CheatUI : UI
{
    public override UIType Type => UIType.Cheat;
    public List<CheatItemUI> CheatItemUIs = new List<CheatItemUI>();

    public override void OnClosed()
    {
        CheatItemUIs.ForEach(cheat => cheat.ClearData());
        GameManager.PlayerData.Inventory.DisposeEvents();
    }

    public override void OnOpened()
    {
        // 아이템
        var itemTable = TableManager.Instance.ItemTable.ToList().OrderBy(item => item.Name);
        List<Dropdown.OptionData> itemOptions = new List<Dropdown.OptionData>();
        foreach (var item in itemTable)
        {
            itemOptions.Add(new Dropdown.OptionData()
            {
                text = $"{item.Name}",
                image = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{item.IconName}"),
            });
        }
        CheatItemUIs[0].SetData(itemOptions);
        CheatItemUIs[1].SetData(itemOptions);

        // 무기
        var weaponTable = TableManager.Instance.WeaponTable.ToList().OrderBy(weapon => weapon.Name);
        List<Dropdown.OptionData> weaponOptions = new List<Dropdown.OptionData>();
        foreach (var weapon in weaponTable)
        {
            weaponOptions.Add(new Dropdown.OptionData()
            {
                text = $"{weapon.Name}",
                image = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{weapon.IconName}"),
            });
        }
        CheatItemUIs[2].SetData(weaponOptions);


        GameManager.PlayerData.Inventory.OnItemAdd += (itemIndex, itemCount) =>
        {
            var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);

            Debug.LogWarning($"{itemData.Name}를 {itemCount}만큼 넣었다!");
        };
        GameManager.PlayerData.Inventory.OnWeaponAdd += (weaponIndex) =>
        {
            var itemData = TableManager.Instance.WeaponTable.Find(item => item.Index == weaponIndex);

            Debug.LogWarning($"{itemData.Name}를 넣었다!");
        };

        GameManager.PlayerData.Inventory.OnItemRemove += (itemIndex, itemCount) =>
        {
            var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);

            Debug.LogWarning($"{itemData.Name}를 {itemCount}만큼 뺐다!");
        };
        GameManager.PlayerData.Inventory.OnWeaponRemove += (weaponIndex) =>
        {
            var itemData = TableManager.Instance.WeaponTable.Find(item => item.Index == weaponIndex);

            Debug.LogWarning($"{itemData.Name}를 넣었다!");
        };
    }

    public void OnShowInventoryBtnClick()
    {
        GameManager.PlayerData.Inventory.ShowInventoryToString();
    }

    public void OnRefreshGameBtnClick()
    {
        GameManager.Instance.RefreshGame();
    }

    public void OnSavefileDeleteBtnClick()
    {
        GameManager.PlayerData.Delete();
    }

    public void OnCharacterUIOpenBtnClick()
    {
        var ui = GameManager.UISystem.OpenWindow<CharacterUI>(type: UIType.Character);
        ui.SetData(ObjectCode.CHAR_Sparcher);
    }
}
