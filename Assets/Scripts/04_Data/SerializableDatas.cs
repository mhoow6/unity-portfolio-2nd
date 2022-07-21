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
        int maxLevel = TableManager.Instance.CharacterLevelExperienceTable.Count;
        int playerLevel = GameManager.PlayerData.Level;

        int level = Level;
        var levelData = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == level);
        int maxExperience = levelData.MaxExperience;
        int previousExperience = Experience;
        while (maxExperience < gainExperience && levelData.Level != 0)
        {
            // 레벨업이 되는 상황이므로 레벨업
            level++;

            gainExperience -= (maxExperience - previousExperience);
            previousExperience = 0;

            // 만약 만렙에서 레벨업이 되려는 상황이면
            if (level > maxLevel || level > playerLevel)
            {
                level = maxLevel;
                return level;
            }

            levelData = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == level);
            maxExperience = levelData.MaxExperience;
        }
        return level;
    }

    public void LevelUp(int gainExperience)
    {
        int maxLevel = TableManager.Instance.CharacterLevelExperienceTable.Count;
        int playerLevel = GameManager.PlayerData.Level;

        var levelData = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == Level);
        int maxExperience = levelData.MaxExperience;
        int previousExperience = Experience;
        while (maxExperience < gainExperience && levelData.Level != 0)
        {
            // 레벨업이 되는 상황이므로 레벨업
            Level++;
            GameEventSystem.SendEvent(GameEvent.LOBBY_LevelUpCharacter, Code);
            Experience = 0;

            // 만약 만렙에서 레벨업이 되려는 상황이면 or 플레이어 레벨보다 캐릭터 레벨이 높아질려고 하면
            if (Level > maxLevel || Level > playerLevel)
            {
                // 레벨은 그대로 경험치만 최대로
                Level = maxLevel;
                Experience = maxExperience;
                Debug.LogWarning($"[LevelUp]: {Code}가 만렙이여도 레벨업을 시도하고 있습니다.");
                return;
            }
            else if (Level == maxLevel)
            {
                GameEventSystem.SendEvent(GameEvent.GLOBAL_ReachMaxLevelCharacter, Code);
                Debug.LogWarning($"[LevelUp]: {Code}가 만렙이 되었습니다!");
            }
            
            gainExperience -= (maxExperience - previousExperience);
            previousExperience = Experience;

            levelData = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == Level);
            maxExperience = levelData.MaxExperience;
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
        var row = TableManager.Instance.WeaponTable.Find(item => item.Index == weaponIndex);

        Index = row.Index;
        Level = 1;
        Damage = row.BaseDamage;
        Critical = row.BaseCritical;
    }
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

    public List<ItemSlotData> Rewards;

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

    public StageResultData(List<ItemSlotData> rewardList)
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
public class ItemSlotData
{
    public int Index;
    public int Quantity;
    public int SlotIndex;
}

[Serializable]
public class WeaponSlotData
{
    public int Index;
    public int Level;
    public int SlotIndex;
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