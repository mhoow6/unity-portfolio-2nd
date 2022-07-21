using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInitializeSystem : IGameSystem
{
    public void Init(){}

    public void Tick(){}

    #region �÷��̾� ������ �ʱ�ȭ ����
    public void Initalize(PlayerData playerData)
    {
        AddNewbieGift(playerData);
        AddDefaultStagePartyPreset(playerData);
    }

    /// <summary> ó������ ���ӿ� ���� �÷��̾�� ������ ���� </summary>
    void AddNewbieGift(PlayerData playerData)
    {
        if (playerData.NewbieGift == false)
        {
            // �⺻ ĳ���� ����
            playerData.CharacterDatas.Add(new CharacterRecordData()
            {
                Code = ObjectCode.CHAR_Sparcher,
                Level = 1,
                EquipWeaponIndex = 7000,
                Experience = 0,
            });
            playerData.CharacterDatas.Add(new CharacterRecordData()
            {
                Code = ObjectCode.CHAR_Knight,
                Level = 1,
                EquipWeaponIndex = 7001,
                Experience = 0,
            });

            // �⺻ ���� ����
            playerData.Inventory.AddWeapon(7000);
            playerData.Inventory.AddWeapon(7001);

            // ������ ����
            if (playerData.LastEnergyUpdateTime == System.DateTime.MinValue)
                GameManager.EnergyRecoverySystem.AddEnergy(50);

            playerData.NewbieGift = true;
        }
    }

    /// <summary> �������� ��Ƽ ������ �ʱ� ���� </summary>
    void AddDefaultStagePartyPreset(PlayerData playerData)
    {
        // �׽�Ʈ���� ���� ���� ������
        if (playerData.StageRecords.Find(record => record.WorldIdx == 0 && record.StageIdx == 0) == null)
        {
            playerData.StageRecords.Add(new StageRecordData()
            {
                WorldIdx = 0,
                StageIdx = 0,
                CharacterLeader = ObjectCode.CHAR_Sparcher,
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }

        // �������� 1-1 �׽�Ʈ �뵵
        if (playerData.StageRecords.Find(record => record.WorldIdx == 1 && record.StageIdx == 1) == null)
        {
            playerData.StageRecords.Add(new StageRecordData()
            {
                WorldIdx = 1,
                StageIdx = 1,
                CharacterLeader = ObjectCode.CHAR_Sparcher,
                CharacterSecond = ObjectCode.CHAR_Knight,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }
    }
    #endregion
}
