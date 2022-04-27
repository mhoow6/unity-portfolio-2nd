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
        var wasd = player.gameObject.GetComponent<WASDController>();
        GameManager.Instance.InputSystem.Controllers.Remove(m_Joystick);
        if (wasd != null)
            Destroy(wasd);

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
        if (Application.platform != RuntimePlatform.Android)
        {
            var wasd = player.gameObject.AddComponent<WASDController>();
            GameManager.Instance.InputSystem.Controllers.Add(wasd);
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
                        GameManager.Instance.Player.AnimationQueue.Enqueue(aniType);
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
                    () => GameManager.Instance.Player.AnimationQueue.Enqueue(aniType),
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
                    () => GameManager.Instance.Player.AnimationQueue.Enqueue(aniType),
                    string.Empty
                );
        }
    }
}
