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
        // 다시 Canvas의 RenderMode를 Screen Space - Camera
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.UISystem.Canvas.worldCamera = GameManager.Instance.UISystem.UICamera;

        // 컨트롤러 제외
        GameManager.Instance.InputSystem.Controllers.Remove(m_Joystick);
        GameManager.Instance.InputSystem.Controllers.Remove(m_WASDController);
    }

    public override void OnOpened()
    {
        // 컨트롤러 추가
        GameManager.Instance.InputSystem.Controllers.Add(m_Joystick);
        if (Application.platform != RuntimePlatform.Android)
        {
            if (m_WASDController == null)
                m_WASDController = gameObject.AddComponent<WASDController>();

            GameManager.Instance.InputSystem.Controllers.Add(m_WASDController);
        }

        // 카메라 회전 가능
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
