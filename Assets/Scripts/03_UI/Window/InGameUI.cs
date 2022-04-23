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

    Inputable m_WASDController;

    public override void OnClosed()
    {
        // �ٽ� Canvas�� RenderMode�� Screen Space - Camera
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.UISystem.Canvas.worldCamera = GameManager.Instance.UISystem.UICamera;

        // ��Ʈ�ѷ� ����
        GameManager.Instance.InputSystem.Controllers.Remove(m_Joystick);
        GameManager.Instance.InputSystem.Controllers.Remove(m_WASDController);
    }

    public override void OnOpened()
    {
        // ��Ʈ�ѷ� �߰�
        GameManager.Instance.InputSystem.Controllers.Add(m_Joystick);
        if (Application.platform != RuntimePlatform.Android)
        {
            if (m_WASDController == null)
                m_WASDController = gameObject.AddComponent<WASDController>();

            GameManager.Instance.InputSystem.Controllers.Add(m_WASDController);
        }

        // ī�޶� ȸ�� ����
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
