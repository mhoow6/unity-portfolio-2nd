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
        Debug.Log($"{other.gameObject.name}¿Ã »£√‚({++m_CurrentTriggerCount})");

        if (!(m_Data && m_Attacker))
            return;

        if (m_CurrentTriggerCount < m_Data.MaximumHits)
        {
            m_CurrentTriggerCount++;

            Character victim = StageManager.Instance.Monsters.Find(mob => mob.gameObject.Equals(other.gameObject));
            if (victim)
            {
                var damageData = m_Attacker.CalcuateDamage(victim, m_Data.DamageScale);
                victim.Damaged(m_Attacker, damageData.Item1, damageData.Item2);
            }
        }
    }
}
