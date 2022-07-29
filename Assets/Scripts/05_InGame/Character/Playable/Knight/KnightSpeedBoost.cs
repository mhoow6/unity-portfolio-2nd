using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class KnightSpeedBoost : IBuffable
{
    KnightXInputData m_SkillData;
    Knight m_Knight;

    public KnightSpeedBoost(Knight knight)
    {
        var _skillData = Character.GetSkillData(Character.GetXInputDataIndex(ObjectCode.CHAR_Knight));
        var skillData = _skillData as KnightXInputData;

        m_SkillData = skillData;
        m_Knight = knight;
    }

    public void Affect()
    {
        // 이동속도 세팅
        m_Knight.MoveSpeed = m_SkillData.BuffSetSpeed;
    }

    public int Duration()
    {
        return m_SkillData.BuffDuration;
    }

    public void Remove()
    {
        float minSpeed = Character.GetCharacterData(m_Knight.Code, m_Knight.Level, m_Knight.EquipWeaponIndex).Speed;

        // 버프된 속도만 빼준다.
        m_Knight.MoveSpeed = Mathf.Max(minSpeed, m_Knight.MoveSpeed - m_SkillData.BuffSetSpeed);
    }
}
