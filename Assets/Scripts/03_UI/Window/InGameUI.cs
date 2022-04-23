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


    public override void OnClosed()
    {
        // �ΰ��� �ٱ����� ���̽�ƽ�� �ʿ�����Ƿ� �ٽ� Canvas�� RenderMode�� Screen Space - Camera
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.UISystem.Canvas.worldCamera = GameManager.Instance.UISystem.UICamera;
    }

    public override void OnOpened()
    {
        GameManager.Instance.InputSystem.Controller = m_Joystick;
        GameManager.Instance.InputSystem.CameraRotatable = true;

        // Canvas�� RenderMode�� Screen Space - Camera�� ��� ���̽�ƽ�� ���������� �۵����� �ʴ� ���� �߻�
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SkillButtonSetup();
    }

    /// <summary> ���� ĳ������ ���� ��ų��ư�� �����մϴ�. </summary>
    public void SkillButtonSetup()
    {
        
    }
}
