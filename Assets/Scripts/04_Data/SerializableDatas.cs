using System;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

[Serializable]
public class StageRecordData
{
    public int WorldIdx;
    public int StageIdx;
    public bool Clear;

    public ObjectCode CharacterLeader;
    public ObjectCode CharacterSecond;
    public ObjectCode CharacterThird;
}

[Serializable]
public class QuestRecordData
{
    public int QuestIdx;
    public QuestType Type;
    public int SuccessCount;
    public bool Clear;
}

[Serializable]
public class CharacterData
{
    public ObjectCode Code;
    public int Level;
    public int Hp;
    public int Sp;
    public int Damage;
    public int Defense;
    public int Critical;
    public float Speed;
    public int GroggyExhaustion;
    public int GroggyRecoverySpeed;

    public WeaponData EquipWeaponData;
}

[Serializable]
public class CharacterRecordData
{
    public ObjectCode Code;
    public int Level;
    public int EquipWeaponIndex;
    public int Experience;

    public int LevelUpSimulate(int gainExperience)
    {
        int level = Level;
        int maxExperience = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == level).MaxExperience;
        int previousExperience = Experience;
        while (maxExperience < gainExperience)
        {
            // 레벨업이 되는 상황이므로 레벨업
            level++;

            gainExperience -= (maxExperience - previousExperience);
            previousExperience = 0;

            maxExperience = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == level).MaxExperience;
        }
        return level;
    }

    public void LevelUp(int gainExperience)
    {
        int maxExperience = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == Level).MaxExperience;
        int previousExperience = Experience;
        while (maxExperience < gainExperience)
        {
            // 레벨업이 되는 상황이므로 레벨업
            Level++;
            Experience = 0;

            gainExperience -= (maxExperience - previousExperience);

            previousExperience = Experience;
            maxExperience = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == Level).MaxExperience;
        }
        Experience += Mathf.Abs(gainExperience);
        Debug.LogWarning($"[LevelUp]: {Code}는 레벨:{Level}과 {Experience}가 되었습니다.");
    }
}

[Serializable]
public struct WeaponData
{
    public int Index;
    public int Level;
    public float Damage;
    public float Critical;

    public WeaponData(int weaponIndex)
    {
        var row = TableManager.Instance.ItemTable.Find(item => item.Index == weaponIndex);

        if (JsonManager.Instance.JsonDatas.TryGetValue(weaponIndex, out var json))
        {
            var weaponData = json as Weaponable;
            Critical = weaponData.WeaponCritical;
        }
        else
            Critical = 0;

        Index = row.Index;
        Level = 1;
        Damage = row.Point;
        
    }
}

[Serializable]
public struct WeaponRecordData
{
    public int Index;
    public int Level;
}

[Serializable]
public struct GameSettings
{
    public bool AutoTargeting;
    public int TargetFrameRate;
}

[Serializable]
public struct CheatSettings
{
    [Tooltip("플레이어가 무적이 됩니다.")]
    public bool GodMode;

    [Tooltip("플레이어가 한 방에 적을 죽일 수 있습니다.")]
    public bool OneShotKill;

    [Tooltip("100% 확률로 HP, SP 회복 아이템을 얻습니다.")]
    public bool DropRecovery100;

    [Tooltip("SP나 쿨타임 소비 없이 스킬을 쓸 수 있습니다.")]
    public bool FreeSkill;

    [Tooltip("치트 창을 열 수 있습니다.")]
    public bool CheatUI;
}

[Serializable]
public class StageResultData
{
    public int WorldIdx;
    public int StageIdx;
    public bool Clear;

    public int Score
    {
        get => m_Score;
        set
        {
            m_Score = value;
            PlayerGetExperience = (int)Mathf.Floor(Score * 0.1f);
            CharacterGetExperience = (int)Mathf.Floor(Score * 0.5f);
        }
    }

    public int MonsterKillCount;
    public int BossKillCount;

    public int Gold;
    public int Combo;

    public int PlayerGetExperience { get; private set; }
    public int CharacterGetExperience { get; private set; }

    public StagePlayerData PlayerRecord;
    public List<StageCharacterData> CharacterRecords;

    public List<ItemData> Rewards;

    public TimeSpan Duration
    {
        get
        {
            return StageEndTime - StageStartTime;
        }
    }
    public readonly DateTime StageStartTime;
    public DateTime StageEndTime;

    int m_Score;

    public StageResultData(List<ItemData> rewardList)
    {
        WorldIdx = 0;
        StageIdx = 0;
        Clear = false;
        Rewards = rewardList;
        Gold = 0;
        Score = 0;
        Combo = 0;
        StageStartTime = DateTime.Now;
        StageEndTime = default(DateTime);
        MonsterKillCount = 0;
        BossKillCount = 0;
        CharacterRecords = new List<StageCharacterData>();
        PlayerRecord = new StagePlayerData();
    }
}

[Serializable]
public class ItemData
{
    public int Index;
    public int Quantity;
}

public struct StagePlayerData
{
    public int Level;
    public int Experience;
}

public struct StageCharacterData
{
    public ObjectCode Code;
    public int Level;
    public int Experience;
}