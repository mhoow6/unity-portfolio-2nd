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

        // �ٽ� Canvas�� RenderMode�� Screen Space - Camera
        GameManager.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.UISystem.Canvas.worldCamera = GameManager.UISystem.UICamera;

        // ��Ʈ�ѷ� ����
        GameManager.InputSystem.Controllers.Remove(m_Joystick);

        // ������ ������ Ű���� ��Ʈ�ѷ��� �޷�����
        var wasd = player.gameObject.GetComponent<KeyboardController>();
        if (wasd != null)
        {
            Destroy(wasd);

            // Ŀ�� �ٽ� ���̰� �ϱ�
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


        // ������ ĳ���� ���ۺҰ���
        player.Controlable = false;

        // ī�޶� ȸ�� �Ұ���
        GameManager.InputSystem.CameraRotatable = false;
    }

    public override void OnOpened()
    {
        var player = StageManager.Instance.Player;

        // ��Ʈ�ѷ� �߰�
        GameManager.InputSystem.Controllers.Add(m_Joystick);

        // �����Ϳ��� Ű���� ��Ʈ�� �����ϰ�
        if (Application.platform != RuntimePlatform.Android)
        {
            var wasd = player.gameObject.AddComponent<KeyboardController>();
            GameManager.InputSystem.Controllers.Add(wasd);

            // Ŀ���� ���ǻ� ��ų���
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // ������ ĳ���� ���۰���
        player.Controlable = true;

        // ī�޶� ȸ�� ����
        GameManager.InputSystem.CameraRotatable = true;

        // Canvas�� RenderMode�� Screen Space - Camera�� ��� ���̽�ƽ�� ���������� �۵����� �ʴ� ���� �߻�
        GameManager.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SkillButtonSetup(player.CurrentCharacter);
    }

    /// <summary> ĳ������ ���� ��ų��ư�� �����մϴ�. </summary>
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
