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
