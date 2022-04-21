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

    /// <summary> ���� ĳ������ ���� ��ų��ư�� �����մϴ�. </summary>
    public void SkillButtonSetup()
    {
        
    }
}
