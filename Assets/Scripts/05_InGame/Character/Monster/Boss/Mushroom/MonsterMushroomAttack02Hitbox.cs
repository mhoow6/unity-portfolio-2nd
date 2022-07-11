using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomAttack02Hitbox : MonoBehaviour
{
    Character m_Owner;
    int m_CurrentHitCount;
    int m_MaxHitCount;
    float m_DamageScale;

    public void SetData(Character owner, int maxHitCount, float damageScale)
    {
        m_Owner = owner;

        m_CurrentHitCount = 0;
        m_MaxHitCount = maxHitCount;
        m_DamageScale = damageScale;
    }

    public void ResetHitCount()
    {
        m_CurrentHitCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_CurrentHitCount == m_MaxHitCount)
            return;

        if (other.CompareTag(m_Owner.gameObject.tag))
            return;

        if (other.gameObject.layer.CompareTo(m_Owner.gameObject.layer) != 0)
            return;

        // 투사체와 상호작용하고 싶진 않다
        var victim = other.GetComponent<Character>();
        if (victim == null)
            return;

        m_CurrentHitCount++;

        var damageResult = m_Owner.CalcuateDamage(victim, m_DamageScale);
        victim.Damaged(m_Owner, damageResult.Damage, damageResult.IsCrit);
    }
}
