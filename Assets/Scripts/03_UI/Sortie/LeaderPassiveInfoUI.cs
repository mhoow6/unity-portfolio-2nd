using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class LeaderPassiveInfoUI : Display
{
    [SerializeField] Text m_SkillName;
    [SerializeField] Text m_SkillDescription;

    public void SetData(ObjectCode characterCode)
    {
        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        if (record == null)
        {
            Debug.LogError($"{characterCode}�� �ش��ϴ� ĳ���Ͱ� �÷��̾� �����Ϳ� �����ϴ�.");
            return;
        }

        var data = JsonManager.Instance.JsonDatas[Character.GetPassiveIndex(characterCode)] as SkillDescriptable;
        if (data != null)
        {
            m_SkillName.text = data.Name;
            m_SkillDescription.text = data.Description;
        }
        else
        {
            m_SkillName.text = string.Empty;
            m_SkillDescription.text = string.Empty;
        }
    }
}
