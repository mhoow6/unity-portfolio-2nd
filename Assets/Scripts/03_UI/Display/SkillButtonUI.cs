using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DatabaseSystem;

public class SkillButtonUI : Display, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GameObject m_SpRoot;
    [SerializeField] Text m_SpCost;
    [SerializeField] Image m_SkillIcon;
    public Image CoolTimeBackground;

    Action m_OnClicked;
    Action m_OnExited;

    public void SetData(SkillButtonParam param)
    {
        var texturePath = GameManager.GameDevelopSettings.TextureResourcePath;

        if (param.SkillData.SpCost == 0)
        {
            m_SpRoot.gameObject.SetActive(false);
            m_SpCost.text = $"{param.SkillData.SpCost}";
            m_SkillIcon.sprite = Resources.Load<Sprite>($"{texturePath}/{param.SkillData.IconPath}");
            if (m_SkillIcon.sprite == null)
                m_SkillIcon.sprite = Resources.Load<Sprite>($"{texturePath}/icon_question");
        }
        else
            m_SpRoot.gameObject.SetActive(true);

        if (param.SkillData.CoolTime > 0)
            CoolTimeBackground.gameObject.SetActive(true);
        else
            CoolTimeBackground.gameObject.SetActive(false);

        CoolTimeBackground.fillAmount = 0;

        m_OnClicked = param.OnClick;
        m_OnExited = param.OnExit;
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

public struct SkillButtonParam
{
    public Action OnClick;
    public Action OnExit;
    public Skillable SkillData;
}
