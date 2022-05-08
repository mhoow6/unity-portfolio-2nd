using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : AreaComponent
{
    protected bool m_AutoDisable;
    protected bool m_AutoWall;

    protected void OnTriggerEnter(Collider other)
    {
        // Area를 감싸는 벽 ON
        if (m_AutoWall)
        {
            var parent = StageManager.Instance.Areas.Find(a => a.Index == m_AreaIdx);
            parent.Wall = true;
        }

        OnAreaEnter(other);

        // 트리거는 비활성화 여부
        gameObject.SetActive(!m_AutoDisable);
    }

    protected virtual void OnAreaEnter(Collider other) { }
}