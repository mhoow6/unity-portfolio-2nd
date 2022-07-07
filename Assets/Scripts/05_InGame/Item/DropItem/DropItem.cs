using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

[RequireComponent(typeof(BoxCollider))]
public abstract class DropItem : Item, IPoolable
{
    #region ������Ʈ Ǯ��
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }
    bool m_Poolable;

    public void OnLoad()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }
    #endregion

    public override void Use()
    {
        
    }

    // -----------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        // �÷��̾���� ���� ���ϵ��� ����
        if (!other.CompareTag(sm.Player.CurrentCharacter.tag))
            return;
        
        Use();
        sm.PoolSystem.Release(this);
    }
}