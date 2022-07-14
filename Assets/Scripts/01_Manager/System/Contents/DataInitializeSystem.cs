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
        AddRewardForNewbie(playerData);
        AddDefaultStagePartyPreset(playerData);
    }

    /// <summary> ó������ ���ӿ� ���� �÷��̾�� ������ ���� </summary>
    void AddRewardForNewbie(PlayerData playerData)
    {
        // �⺻ ĳ���� ����
        if (playerData.CharacterDatas.Find(character => character.Code == ObjectCode.CHAR_Sparcher) == null)
        {
            playerData.CharacterDatas.Add(new CharacterRecordData()
            {
                Code = ObjectCode.CHAR_Sparcher,
                Level = 1,
                EquipWeaponCode = ObjectCode.NONE,
                Experience = 0,
            });
        }

        // ������ ����
        if (playerData.LastEnergyUpdateTime == System.DateTime.MinValue)
            GameManager.EnergyRecoverySystem.AddEnergy(99);

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
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }
    }
    #endregion
}
