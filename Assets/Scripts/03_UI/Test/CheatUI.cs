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
    }
}
