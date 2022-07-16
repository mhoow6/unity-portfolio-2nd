using System;
using System.Collections.Generic;
using UnityEngine;

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
    public ObjectCode EquipWeaponCode;
    public int Experience;
}

[Serializable]
public class WeaponData
{
    public ObjectCode Code;
    public float Damage;
    public float Critical;
}

[Serializable]
public class WeaponRecordData
{
    public ObjectCode Code;
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
    [Tooltip("�÷��̾ ������ �˴ϴ�.")]
    public bool GodMode;

    [Tooltip("�÷��̾ �� �濡 ���� ���� �� �ֽ��ϴ�.")]
    public bool OneShotKill;

    [Tooltip("100% Ȯ���� HP, SP ȸ�� �������� ����ϴ�.")]
    public bool DropRecovery100Percent;

    [Tooltip("SP�� ��Ÿ�� �Һ� ���� ��ų�� �� �� �ֽ��ϴ�.")]
    public bool FreeSkill;

    [Tooltip("ġƮ â�� �� �� �ֽ��ϴ�.")]
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

    public List<StageRewardItemData> Rewards;

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

    public StageResultData(List<StageRewardItemData> rewardList)
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
public class StageRewardItemData
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