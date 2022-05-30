using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class PassiveSkill
{
    protected int m_DataIndex;
    protected Skillable m_Data;

    public PassiveSkill(int dataIndex)
    {
        m_DataIndex = dataIndex;
        if (JsonManager.Instance.JsonDatas.TryGetValue(m_DataIndex, out var value))
            m_Data = value as Skillable;
    }

    public virtual void Apply(int skillIndex) { }
}
