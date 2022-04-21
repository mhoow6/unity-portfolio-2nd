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

    public void SetData(bool useSp, Action onClick, string skillIconPath, int spCost = 0)
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

    public void OnPointerDown(PointerEventData eventData)
    {
        m_OnClicked?.Invoke();
    }
}
