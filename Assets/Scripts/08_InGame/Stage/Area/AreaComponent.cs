using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaComponent : MonoBehaviour
{
    [SerializeField] protected int AreaIdx;

    public void SetData(int areaIdx)
    {
        AreaIdx = areaIdx;
    }
}
