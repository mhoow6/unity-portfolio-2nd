using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class CheatItemUI : Display
{
    public Dropdown Dropdown;
    public InputField InputField;
    public CheatType CheatType; 

    public void SetData(List<Dropdown.OptionData> options)
    {
        Dropdown.AddOptions(options);
    }

    public void ClearData()
    {
        Dropdown.ClearOptions();
    }

    public void OnButtonClick()
    {
        switch (CheatType)
        {
            case CheatType.Add:
                if (int.TryParse(InputField.text, out int addCount))
                {
                    var itemData = TableManager.Instance.ItemTable.Find(item => item.Name == Dropdown.itemText.text);
                    GameManager.PlayerData.Inventory.AddItem(itemData.Index, addCount);
                }
                break;
            case CheatType.Remove:
                if (int.TryParse(InputField.text, out int removeCount))
                {
                    var itemData = TableManager.Instance.ItemTable.Find(item => item.Name == Dropdown.itemText.text);
                    GameManager.PlayerData.Inventory.RemoveItem(itemData.Index, removeCount);
                }
                break;
        }
    }
}

public enum CheatType
{
    None,
    Add,
    Remove
}
