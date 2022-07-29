using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class KnightShieldBoost : IBuffable
{
    KnightBInputData m_SkillData;
    IEnumerable<Character> m_Characters;

    public KnightShieldBoost(IEnumerable<Character> playerCharacters)
    {
        var _skillData = Character.GetSkillData(Character.GetBInputDataIndex(ObjectCode.CHAR_Knight));
        var skillData = _skillData as KnightBInputData;

        m_SkillData = skillData;
        m_Characters = playerCharacters;
    }

    public void Affect()
    {
        // 파티전체 방어력 증가
        foreach (var character in m_Characters)
            character.Defense += m_SkillData.BuffIncreaseDef;
    }

    public int Duration()
    {
        return m_SkillData.BuffDuration;
    }

    public void Remove()
    {
        List<int> minDefense = new List<int>();

        // 파티전체 방어력 증가
        foreach (var character in m_Characters)
            minDefense.Add(Character.GetCharacterData(character.Code, character.Level, character.EquipWeaponIndex).Defense);

        // 버프된 방어력만 빼준다.
        int index = 0;
        foreach (var character in m_Characters)
        {
            character.Defense = Mathf.Max(minDefense[index], character.Defense - m_SkillData.BuffIncreaseDef);
            index++;
        }
    }
}
