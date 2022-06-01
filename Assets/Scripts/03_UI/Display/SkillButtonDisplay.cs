using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonDisplay : Display, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GameObject m_SpRoot;
    [SerializeField] Text m_SpCost;
    [SerializeField] Image m_SkillIcon;

    Action m_OnClicked;
    Action m_OnExited;

    public void SetData(Action onClick, Action onExit, string skillIconPath, int spCost = 0)
    {
        var texturePath = GameManager.GameDevelopSettings.TextureResourcePath;

        if (spCost == 0)
        {
            m_SpRoot.gameObject.SetActive(false);
            m_SpCost.text = $"{spCost}";
            m_SkillIcon.sprite = Resources.Load<Sprite>($"{texturePath}/{skillIconPath}");
            if (m_SkillIcon.sprite == null)
                m_SkillIcon.sprite = Resources.Load<Sprite>($"{texturePath}/icon_question");
        }
        else
            m_SpRoot.gameObject.SetActive(true);

        m_OnClicked = onClick;
        m_OnExited = onExit;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_OnClicked?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_OnExited?.Invoke();
    }
}
