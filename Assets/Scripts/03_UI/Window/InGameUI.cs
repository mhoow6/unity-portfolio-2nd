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
        // 인게임 바깥에선 조이스틱이 필요없으므로 다시 Canvas의 RenderMode를 Screen Space - Camera
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.UISystem.Canvas.worldCamera = GameManager.Instance.UISystem.UICamera;
    }

    public override void OnOpened()
    {
        GameManager.Instance.InputSystem.Controller = m_Joystick;
        GameManager.Instance.InputSystem.CameraRotatable = true;

        // Canvas의 RenderMode가 Screen Space - Camera일 경우 조이스틱이 정상적으로 작동하지 않는 문제 발생
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SkillButtonSetup();
    }

    /// <summary> 현재 캐릭터의 따라 스킬버튼을 세팅합니다. </summary>
    public void SkillButtonSetup()
    {
        
    }
}
