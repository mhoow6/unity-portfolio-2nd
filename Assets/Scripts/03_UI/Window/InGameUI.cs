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
        var player = GameManager.Instance.Player;

        // �ٽ� Canvas�� RenderMode�� Screen Space - Camera
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.UISystem.Canvas.worldCamera = GameManager.Instance.UISystem.UICamera;

        // ��Ʈ�ѷ� ����
        GameManager.Instance.InputSystem.Controllers.Remove(m_Joystick);

        // ������ ������ Ű���� ��Ʈ�ѷ��� �޷�����
        var wasd = player.gameObject.GetComponent<KeyboardController>();
        if (wasd != null)
        {
            wasd.DisconnectFromInGameUI();
            Destroy(wasd);

            // Ŀ�� �ٽ� ���̰� �ϱ�
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
            

        // ������ ĳ���� ���ۺҰ���
        player.Controlable = false;

        // ī�޶� ȸ�� �Ұ���
        GameManager.Instance.InputSystem.CameraRotatable = false;
    }

    public override void OnOpened()
    {
        var player = GameManager.Instance.Player;

        // ��Ʈ�ѷ� �߰�
        GameManager.Instance.InputSystem.Controllers.Add(m_Joystick);

        // �����Ϳ��� Ű���� ��Ʈ�� �����ϰ�
        if (Application.platform != RuntimePlatform.Android)
        {
            var wasd = player.gameObject.AddComponent<KeyboardController>();
            wasd.ConnectToInGameUI(m_AttackButton, m_DashButton, m_SkillButton);
            GameManager.Instance.InputSystem.Controllers.Add(wasd);

            // Ŀ���� ���ǻ� ��ų���
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // ������ ĳ���� ���۰���
        player.Controlable = true;

        // ī�޶� ȸ�� ����
        GameManager.Instance.InputSystem.CameraRotatable = true;

        // Canvas�� RenderMode�� Screen Space - Camera�� ��� ���̽�ƽ�� ���������� �۵����� �ʴ� ���� �߻�
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SkillButtonSetup(player.CurrentCharacter);
    }

    /// <summary> ĳ������ ���� ��ų��ư�� �����մϴ�. </summary>
    public void SkillButtonSetup(Character character)
    {
        // �⺻���� ����
        if (character.SkillIndices.TryGetValue(SkillType.Attack, out int attackIndex))
        {
            AniType aniType = character.GetAniType(attackIndex);
            m_AttackButton.SetData
                (
                    false,
                    () =>
                    {
                        GameManager.Instance.Player.AnimationJobs.Enqueue(aniType);
                    },
                    string.Empty
                );
        }

        // ��ų ����
        if (character.SkillIndices.TryGetValue(SkillType.Skill, out int skillIndex))
        {
            AniType aniType = character.GetAniType(skillIndex);
            int spCost = character.GetSpCost(skillIndex);

            m_SkillButton.SetData
                (
                    true,
                    () => GameManager.Instance.Player.AnimationJobs.Enqueue(aniType),
                    string.Empty,
                    spCost
                );
        }

        // �뽬 ����
        if (character.SkillIndices.TryGetValue(SkillType.Dash, out int dashIndex))
        {
            AniType aniType = character.GetAniType(dashIndex);
            m_DashButton.SetData
                (
                    false,
                    () => GameManager.Instance.Player.AnimationJobs.Enqueue(aniType),
                    string.Empty
                );
        }
    }
}
