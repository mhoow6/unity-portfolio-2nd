using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInitializeSystem : IGameSystem
{
    public void Init(){}

    public void Tick(){}

    #region 플레이어 데이터 초기화 과정
    public void Initalize(PlayerData playerData)
    {
        AddNewbieGift(playerData);
        AddDefaultStagePartyPreset(playerData);
    }

    /// <summary> 처음으로 게임에 들어온 플레이어에게 데이터 세팅 </summary>
    void AddNewbieGift(PlayerData playerData)
    {
        if (playerData.NewbieGift == false)
        {
            // 기본 캐릭터 지급
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

            // 기본 무기 지급
            playerData.Inventory.AddWeapon(7000);
            playerData.Inventory.AddWeapon(7001);

            // 에너지 지급
            if (playerData.LastEnergyUpdateTime == System.DateTime.MinValue)
                GameManager.EnergyRecoverySystem.AddEnergy(50);

            playerData.NewbieGift = true;
        }
    }

    /// <summary> 스테이지 파티 프리셋 초기 세팅 </summary>
    void AddDefaultStagePartyPreset(PlayerData playerData)
    {
        // 테스트씬에 들어가기 위한 데이터
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

        // 스테이지 1-1 테스트 용도
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
