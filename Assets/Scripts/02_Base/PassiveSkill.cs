using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class PassiveSkill
{
    protected int m_DataIndex;

    public PassiveSkill(int dataIndex)
    {
        m_DataIndex = dataIndex;
    }

    public virtual void Apply(int skillIndex) { }
}
