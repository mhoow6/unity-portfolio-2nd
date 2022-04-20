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
        // ���� ��ư ����
        m_AttackButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.ATTACK);
        });

        // �뽬 ��ư ����
        m_DashButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.DASH);
        });

        // ��ų ��ư ����
        m_SkillButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.SKILL);
        });

        // ��ų ��ư ����
        m_UltimateButton.SetData(false, () =>
        {
            leader.Animator.SetInteger(leader.ANITYPE_HASHCODE, (int)AniType.ULTIMATE);
        });
    }
}
