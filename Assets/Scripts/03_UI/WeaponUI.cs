using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : ItemUI
{
    public Text WeaponLevel;
    public GameObject EquippedObject;

    public void SetWeaponData(int weaponIndex, int slotIdx)
    {
        m_SlotIdx = slotIdx;

        if (weaponIndex < 7000 && weaponIndex > 8000)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        // ���̺�
        var weaponData = TableManager.Instance.WeaponTable.Find(weapon => weapon.Index == weaponIndex);

        // ����
        var weaponRecord = GameManager.PlayerData.Inventory.FindWeaponBySlotIndex(slotIdx);

        // ������
        ItemIcon.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{weaponData.IconName}");

        // ���� ����
        WeaponLevel.text = $"Lv.{weaponRecord.Level}";

        // ���� �����
        EquippedObject.SetActive(false);
        foreach (var cha in GameManager.PlayerData.CharacterDatas)
        {
            if (cha.EquipWeaponIndex == weaponIndex && cha.EquipWeaponSlotIndex == slotIdx)
            {
                EquippedObject.SetActive(true);
                break;
            }
        }

        // ���� ����
        Stars.ForEach(star => star.gameObject.SetActive(false));
        Stars[weaponData.StarCount - 2].gameObject.SetActive(true);

        // ��� ����
        RarityBackground.color = RarityBackgroundColors[weaponData.StarCount - 2];
    }
}
