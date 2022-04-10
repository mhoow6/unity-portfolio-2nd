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
    public CharacterCode Code;
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
    public WeaponCode Code;
    public int ReinforceLevel;
    public float Damage;
    public float Critical;
}