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
        // Area�� ���δ� �� ON
        if (m_AutoWall)
        {
            var parent = StageManager.Instance.Areas.Find(a => a.Index == m_AreaIdx);
            parent.Wall = true;
        }

        OnAreaEnter(other);

        // Ʈ���Ŵ� ��Ȱ��ȭ ����
        gameObject.SetActive(!m_AutoDisable);
    }

    protected virtual void OnAreaEnter(Collider other) { }
}