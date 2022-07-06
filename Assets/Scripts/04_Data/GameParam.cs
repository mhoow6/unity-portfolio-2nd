using DatabaseSystem;
using System;
using UnityEngine;

public struct DamageResult
{
    public bool IsCrit;
    public int Damage;
}

public struct PreloadParam
{
    public GameObject PreloadPrefab;
    public Action<GameObject> OnProcessCompletedCallback;
}

public struct SkillButtonParam
{
    public Action OnClick;
    public Action OnExit;
    public Skillable SkillData;
}

public struct MoveInParabolaParam
{
    public Vector3 StartPosition;
    public Vector3 EndPosition;
    public float Height;
    public float SimulateTime;
}