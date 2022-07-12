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

    [Tooltip("100% Ȯ���� ��� �������� ����ϴ�.")]
    public bool DropItem100Percent;

    [Tooltip("SP�� ��Ÿ�� �Һ� ���� ��ų�� �� �� �ֽ��ϴ�.")]
    public bool FreeSkill;
}

[Serializable]
public struct StageResultData
{
    public int Score;

    public int MonsterKillCount;
    public int BossKillCount;

    public int Gold;
    public List<StageRewardItemData> Rewards;

    public readonly TimeSpan Duration
    {
        get
        {
            return StageStartTime - StageEndTime;
        }
    }
    public readonly DateTime StageStartTime;
    public DateTime StageEndTime;

    public StageResultData(List<StageRewardItemData> rewardList)
    {
        Rewards = rewardList;
        Gold = 0;
        Score = 0;
        StageStartTime = DateTime.Now;
        StageEndTime = default(DateTime);
        MonsterKillCount = 0;
        BossKillCount = 0;
    }
}

[Serializable]
public class StageRewardItemData
{
    public ObjectCode Code;
    public int Quantity;
}