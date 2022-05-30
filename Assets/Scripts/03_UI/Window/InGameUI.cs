using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class InGameUI : UI
{
    public override UIType Type => UIType.InGame;

    [SerializeField] VirtualJoystick m_Joystick;
    [SerializeField] SkillButtonDisplay m_AttackButton;
    [SerializeField] SkillButtonDisplay m_DashButton;
    [SerializeField] SkillButtonDisplay m_SkillButton;

    public override void OnClosed()
    {
        var player = StageManager.Instance.Player;

        // 다시 Canvas의 RenderMode를 Screen Space - Camera
        GameManager.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.UISystem.Canvas.worldCamera = GameManager.UISystem.UICamera;

        // 컨트롤러 제외
        GameManager.InputSystem.Controllers.Remove(m_Joystick);

        // 에디터 버젼은 키보드 컨트롤러도 달려있음
        var wasd = player.gameObject.GetComponent<KeyboardController>();
        if (wasd != null)
        {
            Destroy(wasd);

            // 커서 다시 보이게 하기
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


        // 유저가 캐릭터 조작불가능
        player.Controlable = false;

        // 카메라 회전 불가능
        GameManager.InputSystem.CameraRotatable = false;
    }

    public override void OnOpened()
    {
        var player = StageManager.Instance.Player;

        // 컨트롤러 추가
        GameManager.InputSystem.Controllers.Add(m_Joystick);

        // 에디터에선 키보드 컨트롤 가능하게
        if (Application.platform != RuntimePlatform.Android)
        {
            var wasd = player.gameObject.AddComponent<KeyboardController>();
            GameManager.InputSystem.Controllers.Add(wasd);

            // 커서도 편의상 잠궈놓자
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // 유저가 캐릭터 조작가능
        player.Controlable = true;

        // 카메라 회전 가능
        GameManager.InputSystem.CameraRotatable = true;

        // Canvas의 RenderMode가 Screen Space - Camera일 경우 조이스틱이 정상적으로 작동하지 않는 문제 발생
        GameManager.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SkillButtonSetup(player.CurrentCharacter);
    }

    /// <summary> 캐릭터의 따라 스킬버튼을 세팅합니다. </summary>
    public void SkillButtonSetup(Character character)
    {
        int attackIndex = Character.GetAttackIndex(character.Code);
        m_AttackButton.SetData(
            () => { GameManager.InputSystem.CharacterAttackInput = true; },
            () => { GameManager.InputSystem.CharacterAttackInput = false; },
            character.GetSkillIconPath(attackIndex),
            character.GetSpCost(attackIndex));

        //int dashIndex = Character.GetDashIndex(character.Code);
        //m_AttackButton.SetData(
        //    () => { GameManager.InputSystem.CharacterDashInput = true; },
        //    () => { GameManager.InputSystem.CharacterDashInput = false; },
        //    character.GetSkillIconPath(dashIndex),
        //    character.GetSpCost(dashIndex));

        //int ultiIndex = Character.GetUltimateIndex(character.Code);
        //m_AttackButton.SetData(
        //    () => { GameManager.InputSystem.CharacterUltiInput = true; },
        //    () => { GameManager.InputSystem.CharacterUltiInput = false; },
        //    character.GetSkillIconPath(ultiIndex),
        //    character.GetSpCost(ultiIndex));
    }
}
