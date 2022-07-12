using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomAttack02Hitbox : MonoBehaviour
{
    Character m_Owner;
    int m_CurrentHitCount;
    int m_MaxHitCount;
    float m_DamageScale;
    List<Character> m_Victims = new List<Character>();

    public void SetData(Character owner, int maxHitCount, float damageScale)
    {
        m_Owner = owner;

        m_CurrentHitCount = 0;
        m_MaxHitCount = maxHitCount;
        m_DamageScale = damageScale;
    }

    public void ResetHitbox()
    {
        m_CurrentHitCount = 0;
        m_Victims.Clear();
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

        // 맞던 캐릭터는 또 맞게 하지 않는다.
        if (m_Victims.Find(cha => cha == victim))
            return;

        m_CurrentHitCount++;
        m_Victims.Add(victim);

        var damageResult = m_Owner.CalcuateDamage(victim, m_DamageScale);
        DamagedParam param = new DamagedParam()
        {
            Attacker = m_Owner,
            Damage = damageResult.Damage,
            IsCrit = damageResult.IsCrit,
        };
        victim.Damaged(param);
    }
}
