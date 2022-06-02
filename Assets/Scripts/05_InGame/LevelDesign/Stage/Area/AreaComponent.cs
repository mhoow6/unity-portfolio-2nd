using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaComponent : MonoBehaviour
{
    [SerializeField] protected int m_AreaIdx;

    public void SetData(int areaIdx)
    {
        m_AreaIdx = areaIdx;
    }
}
