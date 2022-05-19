using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterDisplay : Display
{
    public bool PortraitVisible
    {
        get
        {
            return m_CharacterPortrait.gameObject.activeSelf;
        }
        set
        {
            m_CharacterPortrait.gameObject.SetActive(true);
        }
    }
    public int CharacterLevel => int.Parse(m_CharacterLevel.text);

    [SerializeField] Image m_CharacterPortrait;
    [SerializeField] Text m_CharacterLevel;

    public void SetData(ObjectCode characterCode)
    {
        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        if (record != null)
            m_CharacterLevel.text = $"Lv. {m_CharacterLevel.text}";
        else
        {
            Debug.LogError($"{characterCode}에 해당하는 캐릭터가 플레이어 데이터에 없습니다.");
            PortraitVisible = false;
        }
            
    }
}
