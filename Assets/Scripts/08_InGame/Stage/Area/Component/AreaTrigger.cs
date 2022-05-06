using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : AreaComponent
{
    protected void OnTriggerEnter(Collider other)
    {
        // Area�� ���δ� �� ON
        var parent = StageManager.Instance.Areas.Find(a => a.Index == m_AreaIdx);
        parent.Wall = true;

        OnAreaEnter(other);
    }

    protected virtual void OnAreaEnter(Collider other) { }
}