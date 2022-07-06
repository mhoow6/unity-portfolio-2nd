using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

[RequireComponent(typeof(BoxCollider))]
public abstract class DropItem : Item, IPoolable
{
    #region 오브젝트 풀링
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

        // 벽에 닿으면 그 자리에 멈추도록 해야함
        

        // 플레이어말고는 먹지 못하도록 하자
        if (!other.CompareTag(sm.Player.CurrentCharacter.tag))
            return;
        
        Use();
        sm.PoolSystem.Release(this);
    }
}
