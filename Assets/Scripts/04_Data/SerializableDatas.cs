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
    public int EquipWeaponSlotIndex;
    public int Experience;

    /// <returns>value: 레벨업 시 달성하는 레벨</returns>
    public int LevelUpSimulate(int gainExperience)
    {
        int maxLevel = TableManager.Instance.CharacterLevelExperienceTable.Count;
        int playerLevel = GameManager.PlayerData.Level;

        int level = Level;
        var levelData = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == level);
        int maxExperience = levelData.MaxExperience;
        int previousExperience = Experience;
        while (maxExperience < (previousExperience + gainExperience) && levelData.Level != 0)
        {
            // 레벨업이 되는 상황이므로 레벨업
            level++;

            gainExperience -= (maxExperience - previousExperience);
            previousExperience = 0;

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
        while (maxExperience < (previousExperience + gainExperience) && levelData.Level != 0)
        {
            // 레벨업이 되는 상황이므로 레벨업
            Level++;
            GameEventSystem.SendEvent(GameEvent.LOBBY_LevelUpCharacter, Code);
            Experience = 0;

            // 만약 만렙에서 레벨업이 되려는 상황이면
            if (Level > maxLevel)
            {
                // 레벨은 그대로 경험치만 최대로
                Level = maxLevel;
                Experience = maxExperience;
                Debug.LogWarning($"[LevelUp]: {Code}가 만렙이여도 레벨업을 시도하고 있습니다.");
                return;
            }

            // 플레이어 레벨보다 캐릭터 레벨이 높아질려고 하면
            if (Level > playerLevel)
            {
                // 이전 레벨과 경험치로 복구
                Level = playerLevel;
                Experience = maxExperience;
                Debug.LogWarning($"[LevelUp]: {Code}가 플레이어보다 레벨이 높아질려고 합니다");
                return;
            }

            if (Level == maxLevel)
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
        Debug.LogWarning($"[CharacterRecordData.LevelUp]: {Code}는 레벨:{Level}과 경험치: {Experience}가 되었습니다.");
    }

    /// <returns>value: 현재 캐릭터가 최대 캐릭터 레벨까지 얻을 수 있는 최대 경험치</returns>
    public int MaxGainExperienceUntilMaxCharacterLevel()
    {
        int result = 0;
        int characterMaxLevel = TableManager.Instance.CharacterLevelExperienceTable.Count;

        for (int i = Level; i <= characterMaxLevel; i++)
        {
            int maxExperience = TableManager.Instance.CharacterLevelExperienceTable[i - 1].MaxExperience;

            if (i == Level)
                result += (maxExperience - Experience);
            else
                result += maxExperience;
        }

        return result;
    }

    /// <returns>value: 현재 캐릭터가 플레이어 레벨까지 얻을 수 있는 최대 경험치</returns>
    public int MaxGainExperienceUntilPlayerLevel()
    {
        int result = 0;
        int playerLevel = GameManager.PlayerData.Level;

        for (int i = Level; i <= playerLevel; i++)
        {
            int maxExperience = TableManager.Instance.CharacterLevelExperienceTable[i - 1].MaxExperience;

            if (i == Level)
                result += (maxExperience - Experience);
            else
                result += maxExperience;
        }

        return result;
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

    [Tooltip("SP나 쿨타임 궁극기를 쓸 수 있습니다.")]
    public bool FreeUlti;
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
            CharacterGetExperience = (int)Mathf.Floor(Score * 0.05f);
        }
    }

    public int MonsterKillCount;
    public int BossKillCount;

    public int Gold;
    public int MaxCombo;

    public int PlayerGetExperience { get; private set; }
    public int CharacterGetExperience { get; private set; }

    public StagePlayerData PlayerRecord;
    public List<StageCharacterData> CharacterRecords;

    public List<RewardData> Rewards;

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

    public StageResultData(List<RewardData> rewardList)
    {
        WorldIdx = 0;
        StageIdx = 0;
        Clear = false;
        Rewards = rewardList;
        Gold = 0;
        Score = 0;
        MaxCombo = 0;
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
public class RewardData
{
    public int Index;
    public int Quantity;
}

readonly public struct ReadOnlyItemSlotData
{
    public int Index { get; }
    public int Quantity { get; }
    public int SlotIndex { get; }

    public ReadOnlyItemSlotData(ItemSlotData data)
    {
        this.Index = data.Index;
        this.Quantity = data.Quantity;
        this.SlotIndex = data.SlotIndex;
    }
}

[Serializable]
public class WeaponSlotData
{
    public int Index;
    public int Level;
    public int Experience;
    public int SlotIndex;
}

readonly public struct ReadOnlyWeaponSlotData
{
    public int Index { get; }
    public int Level { get; }
    public int SlotIndex { get; }
    public int Experience { get; }

    public ReadOnlyWeaponSlotData(WeaponSlotData data)
    {
        this.Index = data.Index;
        this.Level = data.Level;
        this.SlotIndex = data.SlotIndex;
        this.Experience = data.Experience;
    }
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

public struct InventoryUIStoredData
{
    public GameObject GameObject;
    public int ItemIndex;
    public int SlotIdx;
}

public struct ItemExplainUIStoredData
{
    public int ItemIndex;
    public int UseQuantity;
    public int MaxUseQuantity;
}