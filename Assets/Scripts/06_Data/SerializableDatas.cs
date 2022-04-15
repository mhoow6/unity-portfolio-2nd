using System;

[Serializable]
public class StageRecordData
{
    public int WorldIdx;
    public int StageIdx;
    public bool Clear;
}

[Serializable]
public class QuestRecordData
{
    public int QuestIdx;
    public int SuccessCount;
    public bool Clear;
}

[Serializable]
public class CharacterData
{
    public ObjectCode Code;
    public int Level;
    public float Hp;
    public float Sp;
    public float Damage;
    public float Defense;
    public float Critical;

    public WeaponData EquipWeaponData;
}

[Serializable]
public class WeaponData
{
    public ObjectCode Code;
    public float Damage;
    public float Critical;
}