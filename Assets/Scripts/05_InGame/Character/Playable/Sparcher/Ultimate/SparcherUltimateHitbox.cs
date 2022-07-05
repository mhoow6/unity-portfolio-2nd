using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class SparcherUltimateHitbox : MonoBehaviour
{
    SparcherUltiData m_Data;
    Sparcher m_Attacker;
    int m_CurrentTriggerCount;

    private void Awake()
    {
        if (JsonManager.Instance.JsonDatas.TryGetValue(Character.GetUltimateIndex(ObjectCode.CHAR_Sparcher), out JsonDatable data))
            m_Data = data as SparcherUltiData;

        if (StageManager.Instance)
            m_Attacker = StageManager.Instance.Player.CurrentCharacter as Sparcher;
    }

    private void OnEnable()
    {
        m_CurrentTriggerCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(m_Data && m_Attacker))
            return;

        // 시전자 자신은 맞지 않음
        if (other.CompareTag(m_Attacker.tag))
            return;

        // BaseObject끼리의 충돌만 허용
        if (other.gameObject.layer.CompareTo(m_Attacker.gameObject.layer) != 0)
            return;

        if (m_CurrentTriggerCount < m_Data.MaximumHits)
        {
            // 몬스터에게맞 충돌이 허용
            Character victim = StageManager.Instance.Monsters.Find(mob => mob.gameObject.Equals(other.gameObject));
            if (victim)
            {
                m_CurrentTriggerCount++;
                m_Attacker.Target = victim;
                var damageData = m_Attacker.CalcuateDamage(victim, m_Data.DamageScale);
                victim.Damaged(m_Attacker, damageData.Damage, damageData.IsCrit);
            }
        }
    }
}
