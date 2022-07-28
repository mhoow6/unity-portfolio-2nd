using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

[RequireComponent(typeof(BoxCollider))]
public abstract class DropItem : Loot, IPoolable
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

    public override void Pickup()
    {
        
    }

    // -----------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        // 플레이어말고는 먹지 못하도록 하자
        if (!other.CompareTag(sm.Player.CurrentCharacter.tag))
            return;
        
        Pickup();
        sm.PoolSystem.Release(this);
    }
}
