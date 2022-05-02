using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : MonoBehaviour
{
    protected int m_AreaIdx;

    public void SetData(int areaIdx)
    {
        m_AreaIdx = areaIdx;
    }

    protected void OnTriggerEnter(Collider other)
    {
        OnAreaEnter(other);
    }

    protected virtual void OnAreaEnter(Collider other) { }
}