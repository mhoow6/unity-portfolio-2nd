using System;
using System.Collections.Generic;

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
    public bool GodMode;
    public bool OneShotKill;
    public bool DropRecoveryItemClearly;
}

[Serializable]
public struct StageResultData
{
    public int Gold;
    public int Score;
    public List<StageRewardItemData> Rewards;
    public TimeSpan Duration;

    public StageResultData(List<StageRewardItemData> rewardList)
    {
        Rewards = rewardList;
        Gold = 0;
        Score = 0;
        Duration = default(TimeSpan);
    }
}

[Serializable]
public class StageRewardItemData
{
    public ObjectCode Code;
    public int Quantity;
}