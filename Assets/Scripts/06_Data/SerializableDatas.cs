using System;
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
public class WeaponData
{
    public ObjectCode Code;
    public float Damage;
    public float Critical;
}

[Serializable]
public class StageSet
{
    public int WorldIdx;
    public int StageIdx;
}

[Serializable]
public struct SceneLoadData
{
    public int BuildIndex;
    public StageSet Set;
}

[Serializable]
public struct SceneData
{
    public Camera Camera;
    public Light DirectionalLight;
    public SceneType CurrentSceneType;
}