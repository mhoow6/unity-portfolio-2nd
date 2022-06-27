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
    public List<SkillButtonStack> SkillStacks = new List<SkillButtonStack>();
    Stack<SkillButtonStack> m_SkillStacks = new Stack<SkillButtonStack>();

    Action m_OnClicked;
    Action m_OnExited;

    public void SetData(SkillButtonParam param)
    {
        var texturePath = GameManager.GameDevelopSettings.TextureResourcePath;

        // SP표기
        if (param.SkillData.SpCost == 0)
            m_SpRoot.gameObject.SetActive(false);
        else
        {
            m_SpRoot.gameObject.SetActive(true);
            m_SpCost.text = $"{param.SkillData.SpCost}";
        }

        // 스킬 아이콘
        m_SkillIcon.sprite = Resources.Load<Sprite>($"{texturePath}/{param.SkillData.IconPath}");
        if (m_SkillIcon.sprite == null)
            m_SkillIcon.sprite = Resources.Load<Sprite>($"{texturePath}/icon_question");

        // 쿨타임 백그라운드
        CoolTimeBackground.fillAmount = 0;
        if (param.SkillData.CoolTime > 0)
            CoolTimeBackground.gameObject.SetActive(true);
        else
            CoolTimeBackground.gameObject.SetActive(false);

        // 스택표기
        SkillStacks.ForEach((element) => { element.gameObject.SetActive(false); });
        if (param.SkillData.Stack != 0)
        {
            int stackCount = param.SkillData.Stack;
            for (int i = 0; i < stackCount; i++)
            {
                SkillStacks[i].gameObject.SetActive(true);
                m_SkillStacks.Push(SkillStacks[i]);
            }
        }

        m_OnClicked = param.OnClick;
        m_OnExited = param.OnExit;
    }

    public void OnStackConsume()
    {
        var cur = m_SkillStacks.Pop();
        float r = cur.Image.color.r;
        float g = cur.Image.color.g;
        float b = cur.Image.color.b;
        cur.Image.color = new Color(r, g, b, 0.5f);
    }

    public void OnStackCharge(int currentStackCount)
    {
        var stack = SkillStacks[currentStackCount - 1];

        float r = stack.Image.color.r;
        float g = stack.Image.color.g;
        float b = stack.Image.color.b;
        stack.Image.color = new Color(r, g, b, 1);

        m_SkillStacks.Push(stack);
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
