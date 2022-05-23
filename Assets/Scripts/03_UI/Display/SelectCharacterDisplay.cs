using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

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

    [SerializeField] Image m_CharacterPortrait;
    [SerializeField] Text m_CharacterLevel;

    public void SetData(ObjectCode characterCode)
    {
        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        var row = TableManager.Instance.CharacterTable.Find(c => c.Code == characterCode);

        if (record != null)
        {
            m_CharacterLevel.text = $"Lv. {m_CharacterLevel.text}";
            m_CharacterPortrait.sprite = Resources.Load<Sprite>($"{GameManager.Instance.Config.TextureResourcePath}/{row.PortraitName}");
        }
            
        else
        {
            Debug.LogError($"{characterCode}에 해당하는 캐릭터가 플레이어 데이터에 없습니다.");
            PortraitVisible = false;
        }
            
    }
}
