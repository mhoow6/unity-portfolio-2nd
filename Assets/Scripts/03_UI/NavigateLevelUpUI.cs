using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using System.Linq;

public class NavigateLevelUpUI : UI
{
    public override UIType Type => UIType.NavigateLevelUp;

    public Text GainExperience;
    public Text CurrentLevel;
    public Text AfterLevel;

    public GameObject ItemsParent;
    public GameObject ItemPrefab;
    public List<Color> ItemBackgroundColors = new List<Color>();

    public Text ConsumeCount;

    ItemUI m_SelectedItem;
    int m_ConsumeCount
    {
        get
        {
            return int.Parse(ConsumeCount.text);
        }
        set
        {
            ConsumeCount.text = value.ToString();
        }
    }


    public override void OnClosed(){}

    public override void OnOpened(){}

    public void OnCloseWindowBtnClick()
    {
        GameManager.UISystem.CloseWindow();
    }

    public void SetData(ObjectCode levelUpCharacter)
    {
        // 캐릭터 레벨업 아이템 생성
        var levelupItemDatas = TableManager.Instance.ItemTable.Where(row => row.Type == ItemType.CharacterLevelUpChip);
        foreach (var data in levelupItemDatas)
        {
            var userItem = GameManager.PlayerData.Inventory.Items.Find(item => item.Index == data.Index);
            if (userItem != null)
            {
                var inst = Instantiate(ItemPrefab, ItemsParent.transform);
                int userItemCount = userItem.Quantity;

                var itemUI = inst.GetComponent<ItemUI>();
                var itemUIToggle = inst.AddComponent<Toggle>();

                itemUI.SetData(data.Index, userItemCount);
                itemUIToggle.onValueChanged.AddListener((bool isOn) =>
                {
                    if (isOn)
                    {
                        m_SelectedItem = itemUI;
                        m_ConsumeCount = 1;

                        // ItemUI 백그라운드 색상 변경
                        itemUI.ItemFrame.color = ItemBackgroundColors[1];

                        // 레벨업 UI 요소들 변경
                        GainExperience.text = data.Point.ToString();
                        CurrentLevel.text = GameManager.PlayerData.Level.ToString();
                        //AfterLevel.text
                    }
                    else
                    {
                        // ItemUI 백그라운드 색상 변경
                        itemUI.ItemFrame.color = ItemBackgroundColors[0];
                    }

                });
            }
        }
    }

    public void SetData(int levelupWeaponIndex)
    {

    }
}
