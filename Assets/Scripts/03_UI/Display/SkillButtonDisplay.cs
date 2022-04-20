using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonDisplay : Display, IPointerDownHandler
{
    [SerializeField] GameObject m_SpRoot;
    [SerializeField] Text m_SpCost;
    [SerializeField] Image m_SkillIcon;

    Action m_OnClicked;

    // TODO: 스킬 데이터 설계 끝나면 삭제
    public void SetData(bool useSp, Action onClick, string skillIconPath = null, int spCost = 0)
    {
        if (useSp)
        {
            m_SpRoot.gameObject.SetActive(false);
            m_SpCost.text = $"{spCost}";
            m_SkillIcon.sprite = Resources.Load<Sprite>($"02_Texture/{skillIconPath}");
        }
        else
            m_SpRoot.gameObject.SetActive(true);

        m_OnClicked = onClick;
    }

    public void SetData(SkillData data, Action onClick)
    {
        if (data.SpCost > 0)
        {
            m_SpRoot.gameObject.SetActive(false);
            m_SpCost.text = $"{data.SpCost}";
            m_SkillIcon.sprite = Resources.Load<Sprite>($"02_Texture/{data.IconPath}");
        }
        else
            m_SpRoot.gameObject.SetActive(true);

        m_OnClicked = onClick;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_OnClicked?.Invoke();
    }
}
