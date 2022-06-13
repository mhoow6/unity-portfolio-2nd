using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class SparcherPassiveSkill : PassiveSkill
{
    SparcherPassiveSkillData m_Data;

    public SparcherPassiveSkill(int dataIndex) : base(dataIndex)
    {
        if (JsonManager.Instance.JsonDatas.TryGetValue(m_DataIndex, out var value))
            m_Data = value as SparcherPassiveSkillData;
    }

    public override void Apply(int skillIndex)
    {
        
    }
}
