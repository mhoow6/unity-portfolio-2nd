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

    public ToggleGroup ItemsParent;
    public GameObject ItemPrefab;
    public List<Color> ItemBackgroundColors = new List<Color>();

    public Text ConsumeCountText;
    public Slider ConsumeSlider;

    public List<GameObject> Groups = new List<GameObject>();

    public Button ConsumePlusButton;
    public Button ConsumeMinusButton;
    public GameObject MaxObject;

    ObjectCode m_SelectedCharacter;
    int m_SelectedWeaponIndex;

    int m_ConsumeItemIndex;
    int m_ConsumeItemCount;

    public override void OnClosed(){}

    public override void OnOpened(){}

    public void OnCloseWindowBtnClick()
    {
        GameManager.UISystem.CloseWindow();
    }

    public void SetData(ObjectCode levelUpCharacter)
    {
        // 찌거기 제거
        for (int i = 0; i < ItemsParent.transform.childCount; i++)
        {
            var child = ItemsParent.transform.GetChild(i);

            Destroy(child.gameObject);
        }

        m_SelectedCharacter = levelUpCharacter;
        m_SelectedWeaponIndex = -1;

        // 캐릭터 레벨업 아이템 생성
        var levelupItemDatas = TableManager.Instance.ItemTable.Where(row => row.Type == ItemType.CharacterLevelUpChip);

        // 별의 갯수 기준 정렬
        levelupItemDatas = levelupItemDatas.ToList().OrderBy(item => item.StarCount);

        // 인벤토리에 레벨업 아이템이 없을 경우?
        bool levelupItemExist = false;
        foreach (var data in levelupItemDatas)
        {
            var userItem = GameManager.PlayerData.Inventory.FindItemByIndex(data.Index);
            if (userItem == null)
                levelupItemExist |= false;
            else
            {
                levelupItemExist |= true;
                break;
            }
        }
        if (levelupItemExist == false)
        {
            // 상호작용할 수 있는 UI를 다 꺼준다.
            Groups.ForEach(group => group.gameObject.SetActive(false));
            return;
        }
        Groups.ForEach(group => group.gameObject.SetActive(true));

        int count = 0;
        foreach (var data in levelupItemDatas)
        {
            var userItem = GameManager.PlayerData.Inventory.FindItemByIndex(data.Index);
            if (userItem != null)
            {
                var inst = Instantiate(ItemPrefab, ItemsParent.transform);
                int userItemCount = userItem.Quantity;

                var itemUI = inst.GetComponent<ItemUI>();
                var itemUIToggle = inst.AddComponent<Toggle>();

                itemUI.SetItemData(data.Index, userItemCount, userItem.SlotIndex);
                itemUIToggle.onValueChanged.AddListener((bool isOn) =>
                {
                    if (isOn)
                    {
                        var characterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == levelUpCharacter);
                        m_ConsumeItemIndex = data.Index;

                        // ItemUI 백그라운드 색상 변경
                        itemUI.ItemFrame.color = ItemBackgroundColors[1];

                        // 현재 UI 요소들 변경
                        GainExperience.text = data.Point.ToString();
                        CurrentLevel.text = characterRecord.Level.ToString();

                        int afterLevel = characterRecord.LevelUpSimulate(data.Point);

                        if (afterLevel == -1)
                            afterLevel = TableManager.Instance.CharacterLevelExperienceTable.Count;

                        AfterLevel.text = afterLevel.ToString();

                        // 슬라이더
                        ConsumeSlider.minValue = 0;
                        ConsumeSlider.maxValue = userItem.Quantity;

                        // 아이템 소비량 표기
                        ConsumeSlider.value = 1;
                        ConsumeCountText.text = ConsumeSlider.value.ToString();
                    }
                    else
                    {
                        // ItemUI 백그라운드 색상 변경
                        itemUI.ItemFrame.color = ItemBackgroundColors[0];
                    }

                });
                itemUIToggle.group = ItemsParent;

                if (count == 0)
                {
                    itemUIToggle.isOn = true;
                    itemUIToggle.onValueChanged?.Invoke(true);
                }
                    

                count++;
            }
        }
    }

    public void SetData(int levelupWeaponIndex)
    {
        m_SelectedCharacter = ObjectCode.NONE;
        m_SelectedWeaponIndex = levelupWeaponIndex;
    }

    public void ConsumeSliderOnValueChanged()
    {
        ConsumePlusButton.interactable = true;
        ConsumeSlider.interactable = true;
        MaxObject.SetActive(false);

        m_ConsumeItemCount = (int)Mathf.Floor(ConsumeSlider.value);

        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == m_ConsumeItemIndex);
        var record = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == m_SelectedCharacter);
        int afterLevel = record.LevelUpSimulate(itemData.Point * m_ConsumeItemCount);
        int afterafterLevel = record.LevelUpSimulate(itemData.Point * (m_ConsumeItemCount + 1));

        // 다음에 경험치 칩을 1개 더 쓰면 더 이상 레벨업을 하지 못하는 경우가 발생한다.
        if (afterafterLevel == -1)
        {
            ConsumePlusButton.interactable = false;
            ConsumeSlider.interactable = false;

            MaxObject.SetActive(true);
        }

        ConsumeCountText.text = $"{m_ConsumeItemCount}";
        AfterLevel.text = afterLevel.ToString();
    }

    public void OnConsumePlusBtnDown()
    {
        if (ConsumePlusButton.interactable)
            ConsumeSlider.value++;
    }

    public void OnConsumePlusBtnHold()
    {
        if (ConsumePlusButton.interactable)
        {
            for (int i = 0; i < 10; i++)
                ConsumeSlider.value++;
        }
        
    }

    public void OnConsumeMinusBtnDown()
    {
        if (ConsumeMinusButton.interactable)
            ConsumeSlider.value--;
    }

    public void OnConsumeMinusBtnHold()
    {
        if (ConsumeMinusButton.interactable)
        {
            for (int i = 0; i < 10; i++)
                ConsumeSlider.value--;
        }
        
    }

    public void OnConfirmButtonClick()
    {
        if (m_SelectedCharacter != ObjectCode.NONE)
        {
            var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == m_ConsumeItemIndex);
            var record = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == m_SelectedCharacter);

            record.LevelUp(itemData.Point * m_ConsumeItemCount);
            GameEventSystem.SendEvent(GameEvent.LOBBY_ShowCharacter, m_SelectedCharacter);
            GameManager.PlayerData.Inventory.RemoveItem(m_ConsumeItemIndex, m_ConsumeItemCount);

        }
        else if (m_SelectedWeaponIndex != -1)
        {

        }
        
    }
}
