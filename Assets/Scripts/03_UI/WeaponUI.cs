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

        // 테이블
        var weaponData = TableManager.Instance.WeaponTable.Find(weapon => weapon.Index == weaponIndex);

        // 유저
        var weaponRecord = GameManager.PlayerData.Inventory.FindWeaponBySlotIndex(slotIdx);

        // 아이콘
        ItemIcon.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{weaponData.IconName}");

        // 무기 레벨
        WeaponLevel.text = $"Lv.{weaponRecord.Level}";

        // 무기 장비중
        EquippedObject.SetActive(false);
        foreach (var cha in GameManager.PlayerData.CharacterDatas)
        {
            if (cha.EquipWeaponIndex == weaponIndex && cha.EquipWeaponSlotIndex == slotIdx)
            {
                EquippedObject.SetActive(true);
                break;
            }
        }

        // 별의 갯수
        Stars.ForEach(star => star.gameObject.SetActive(false));
        Stars[weaponData.StarCount - 2].gameObject.SetActive(true);

        // 배경 색상
        RarityBackground.color = RarityBackgroundColors[weaponData.StarCount - 2];
    }
}
