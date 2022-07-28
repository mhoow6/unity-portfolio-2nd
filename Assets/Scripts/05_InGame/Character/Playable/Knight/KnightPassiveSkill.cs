using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightPassiveSkill : PassiveSkill
{
    KnightPassiveSkillData m_Data;

    public KnightPassiveSkill(int dataIndex) : base(dataIndex)
    {
        if (JsonManager.Instance.JsonDatas.TryGetValue(m_DataIndex, out var value))
            m_Data = value as KnightPassiveSkillData;
    }

    public override void Apply(int skillIndex)
    {
        var characters = StageManager.Instance.Player.Characters;
        foreach (var character in characters)
            character.Defense = character.Defense + (int)(character.Defense * m_Data.IncreaseDefenseRatio);
    }
}
