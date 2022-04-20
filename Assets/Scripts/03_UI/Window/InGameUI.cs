using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : UI
{
    public override UIType Type => UIType.InGame;

    [SerializeField] VirtualJoystick m_Joystick;
    [SerializeField] SkillButtonDisplay m_AttackButton;
    [SerializeField] SkillButtonDisplay m_DashButton;
    [SerializeField] SkillButtonDisplay m_SkillButton;
    [SerializeField] SkillButtonDisplay m_UltimateButton;


    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        GameManager.Instance.InputSystem.Controller = m_Joystick;
        GameManager.Instance.InputSystem.CameraRotatable = true;

        SkillButtonSetup();
    }

    public void SkillButtonSetup()
    {
        var leader = GameManager.Instance.Player.CurrentCharacter;
        // 공격 버튼 세팅
        m_AttackButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.ATTACK);
        });

        // 대쉬 버튼 세팅
        m_DashButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.DASH);
        });

        // 스킬 버튼 세팅
        m_SkillButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.SKILL);
        });

        // 스킬 버튼 세팅
        m_UltimateButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.ULTIMATE);
        });
    }
}
