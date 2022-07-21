using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

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
        var itemTable = TableManager.Instance.ItemTable;
        List<Dropdown.OptionData> itemOptions = new List<Dropdown.OptionData>();
        foreach (var item in itemTable)
        {
            itemOptions.Add(new Dropdown.OptionData()
            {
                text = $"{item.Name}",
                image = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{item.IconName}"),
            });
        }

        foreach (var cheatItem in CheatItemUIs)
        {
            cheatItem.SetData(itemOptions);
        }

        GameManager.PlayerData.Inventory.OnItemAdd += (itemIndex, itemCount) =>
        {
            var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);

            Debug.LogWarning($"{itemData.Name}를 {itemCount}만큼 넣었다!");
        };

        GameManager.PlayerData.Inventory.OnItemRemove += (itemIndex, itemCount) =>
        {
            var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);

            Debug.LogWarning($"{itemData.Name}를 {itemCount}만큼 뺐다!");
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
