using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;

public class SelectCharacterDisplay : Display, IPointerDownHandler
{
    public bool PortraitVisible
    {
        get
        {
            return m_CharacterPortrait.gameObject.activeSelf && m_CharacterLevel.gameObject.activeSelf;
        }
        set
        {
            m_CharacterPortrait.gameObject.SetActive(value);
            m_CharacterLevel.gameObject.SetActive(value);
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
            m_CharacterLevel.text = $"Lv. {record.Level}";
            m_CharacterPortrait.sprite = Resources.Load<Sprite>($"{GameManager.Config.TextureResourcePath}/{row.PortraitName}");
        }
            
        else
        {
            Debug.Log($"{characterCode}�� �ش��ϴ� ĳ���Ͱ� �÷��̾� �����Ϳ� �����ϴ�.");
            PortraitVisible = false;
        }
            
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // UNDONE: Ŭ�� �� ĳ���� ������ â
    }
}
