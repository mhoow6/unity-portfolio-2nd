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

    InputProvider m_WASDController;

    public override void OnClosed()
    {
        // �ٽ� Canvas�� RenderMode�� Screen Space - Camera
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.UISystem.Canvas.worldCamera = GameManager.Instance.UISystem.UICamera;

        // ��Ʈ�ѷ� ����
        GameManager.Instance.InputSystem.Controllers.Remove(m_Joystick);
        if (m_WASDController != null)
            GameManager.Instance.InputSystem.Controllers.Remove(m_WASDController);

        // ������ ĳ���� ���ۺҰ���
        GameManager.Instance.Player.Controlable = false;

        // ī�޶� ȸ�� �Ұ���
        GameManager.Instance.InputSystem.CameraRotatable = false;
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

        // ������ ĳ���� ���۰���
        GameManager.Instance.Player.Controlable = true;

        // ī�޶� ȸ�� ����
        GameManager.Instance.InputSystem.CameraRotatable = true;

        // Canvas�� RenderMode�� Screen Space - Camera�� ��� ���̽�ƽ�� ���������� �۵����� �ʴ� ���� �߻�
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SkillButtonSetup(GameManager.Instance.Player.CurrentCharacter);
    }

    /// <summary> ���� ĳ������ ���� ��ų��ư�� �����մϴ�. </summary>
    public void SkillButtonSetup(Character character)
    {
        // �⺻���� ����
        if (character.SkillIndices.TryGetValue(Character.SkillType.Attack, out int attackIndex))
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
        if (character.SkillIndices.TryGetValue(Character.SkillType.Skill, out int skillIndex))
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
        if (character.SkillIndices.TryGetValue(Character.SkillType.Dash, out int dashIndex))
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
