using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.UI;

public class ItemExplainUI : UI
{
    public override UIType Type => UIType.ItemExplain;

    public Image ItemFrame;
    public Image ItemBackground;
    public Image ItemIcon;

    public Text ItemName;
    public Text ItemMainDescripition;
    public Text ItemSubDescripition;

    public List<Text> GotItemCount = new List<Text>();
    public Text UseItemCount;
    public List<GameObject> Stars = new List<GameObject>();

    public GameObject UseObject;
    public GameObject NotUseObject;

    ItemExplainUIStoredData m_SelectedItemData;

    public override void OnClosed(){}

    public override void OnOpened(){}

    public void SetData(int itemIndex)
    {
        // 테이블 데이터
        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);
        var itemDescData = TableManager.Instance.ItemExplainTable.Find(item => item.Index == itemIndex);

        // 아이템 레어도 색상
        ItemFrame.color = GameManager.GlobalData.RarityBackgroundColors[itemData.StarCount - 2];
        ItemBackground.color = GameManager.GlobalData.RarityBackgroundColors[itemData.StarCount - 2];

        // 아이템 아이콘
        ItemIcon.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{itemData.IconName}");

        // 아이템 이름
        ItemName.text = $"{itemData.Name}";

        // 아이템 설명
        ItemMainDescripition.text = string.Format(itemDescData.MainExplain, itemData.Point);
        ItemSubDescripition.text = $"{itemDescData.SubExplain}";

        // 아이템 별의 개수
        Stars.ForEach(star => star.gameObject.SetActive(false));
        Stars[itemData.StarCount - 2].gameObject.SetActive(true);

        var userItem = GameManager.PlayerData.Inventory.FindItemByIndex(itemIndex);

        // 데이터 보관
        m_SelectedItemData.ItemIndex = itemIndex;
        m_SelectedItemData.MaxUseQuantity = userItem.Quantity;

        // 유저 보유수량 표기
        UseObject.gameObject.SetActive(false);
        NotUseObject.gameObject.SetActive(false);
        foreach (var itemCount in GotItemCount)
            itemCount.text = $"<color=\"#01C863\">보유 수량 </color><color=\"#FFFFFF\">{userItem.Quantity}</color>";

        // 사용할 수 있는 아이템인가 아닌가
        var inventoryItem = InventoryItem.Get(itemData.Type);
        if (inventoryItem != null)
        {
            // 사용할 아이템 갯수
            UseItemCount.text = 1.ToString();

            UseObject.gameObject.SetActive(true);
        }
        else
            NotUseObject.gameObject.SetActive(true);

    }

    public void OnUseButtonClick()
    {
        
    }

    public void OnMinusButtonClick()
    {
        int value = m_SelectedItemData.UseQuantity - 1;
        if (value < 1)
            m_SelectedItemData.UseQuantity = 1;
        else
            m_SelectedItemData.UseQuantity--;

        UseItemCount.text = $"{m_SelectedItemData.UseQuantity}";
    }

    public void OnPlusButtonClick()
    {
        int value = m_SelectedItemData.UseQuantity + 1;
        if (value > m_SelectedItemData.MaxUseQuantity)
            m_SelectedItemData.UseQuantity = m_SelectedItemData.MaxUseQuantity;
        else
            m_SelectedItemData.UseQuantity++;

        UseItemCount.text = $"{m_SelectedItemData.UseQuantity}";
    }

    public void OnMaxButtonClick()
    {
        m_SelectedItemData.UseQuantity = m_SelectedItemData.MaxUseQuantity;

        UseItemCount.text = $"{m_SelectedItemData.UseQuantity}";
    }

    public void OnWindowCloseBtnClick()
    {
        GameManager.UISystem.CloseWindow(false);
    }
}
