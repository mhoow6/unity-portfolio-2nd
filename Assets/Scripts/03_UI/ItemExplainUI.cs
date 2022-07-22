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
        // ���̺� ������
        var itemData = TableManager.Instance.ItemTable.Find(item => item.Index == itemIndex);
        var itemDescData = TableManager.Instance.ItemExplainTable.Find(item => item.Index == itemIndex);

        // ������ ��� ����
        ItemFrame.color = GameManager.GlobalData.RarityBackgroundColors[itemData.StarCount - 2];
        ItemBackground.color = GameManager.GlobalData.RarityBackgroundColors[itemData.StarCount - 2];

        // ������ ������
        ItemIcon.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{itemData.IconName}");

        // ������ �̸�
        ItemName.text = $"{itemData.Name}";

        // ������ ����
        ItemMainDescripition.text = string.Format(itemDescData.MainExplain, itemData.Point);
        ItemSubDescripition.text = $"{itemDescData.SubExplain}";

        // ������ ���� ����
        Stars.ForEach(star => star.gameObject.SetActive(false));
        Stars[itemData.StarCount - 2].gameObject.SetActive(true);

        var userItem = GameManager.PlayerData.Inventory.FindItemByIndex(itemIndex);

        // ������ ����
        m_SelectedItemData.ItemIndex = itemIndex;
        m_SelectedItemData.MaxUseQuantity = userItem.Quantity;

        // ���� �������� ǥ��
        UseObject.gameObject.SetActive(false);
        NotUseObject.gameObject.SetActive(false);
        foreach (var itemCount in GotItemCount)
            itemCount.text = $"<color=\"#01C863\">���� ���� </color><color=\"#FFFFFF\">{userItem.Quantity}</color>";

        // ����� �� �ִ� �������ΰ� �ƴѰ�
        var inventoryItem = InventoryItem.Get(itemData.Type);
        if (inventoryItem != null)
        {
            // ����� ������ ����
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
