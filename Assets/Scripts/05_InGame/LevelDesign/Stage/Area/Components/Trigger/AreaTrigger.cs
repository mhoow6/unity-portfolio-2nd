using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : AreaComponent
{
    BoxCollider m_Collider;
    protected bool m_AutoDisable;

    protected void Awake()
    {
        m_Collider = GetComponent<BoxCollider>();
        m_Collider.isTrigger = true;
        gameObject.layer = 6;

        OnAwake();
    }

    protected void OnTriggerEnter(Collider other)
    {
        OnAreaEnter(other);

        // 트리거는 비활성화 여부
        if (m_AutoDisable)
            gameObject.SetActive(false);
    }

    protected virtual void OnAwake() { }
    protected virtual void OnAreaEnter(Collider other) { }
}