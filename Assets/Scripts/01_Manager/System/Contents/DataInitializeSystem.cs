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
        AddRewardForNewbie(playerData);
        AddDefaultStagePartyPreset(playerData);
    }

    /// <summary> 처음으로 게임에 들어온 플레이어에게 데이터 세팅 </summary>
    void AddRewardForNewbie(PlayerData playerData)
    {
        // 기본 캐릭터 지급
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

        // 에너지 지급
        if (playerData.LastEnergyUpdateTime == System.DateTime.MinValue)
            GameManager.EnergyRecoverySystem.AddEnergy(99);

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
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }
    }
    #endregion
}
