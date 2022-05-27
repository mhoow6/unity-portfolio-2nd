using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class LeaderPassiveInfoDisplay : Display
{
    [SerializeField] Text m_SkillName;
    [SerializeField] Text m_SkillDescription;

    public void SetData(ObjectCode characterCode)
    {
        var data = JsonManager.Instance.JsonDatas[Character.GetPassiveIndex(characterCode)] as PassiveSkillable;
        if (data != null)
        {
            m_SkillName.text = data.SkillName;
            m_SkillDescription.text = data.SkillDescription;
        }
        else
        {
            m_SkillName.text = string.Empty;
            m_SkillDescription.text = string.Empty;
        }
    }
}
