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

        // 다시 Canvas의 RenderMode를 Screen Space - Camera
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.UISystem.Canvas.worldCamera = GameManager.Instance.UISystem.UICamera;

        // 컨트롤러 제외
        var wasd = player.gameObject.GetComponent<WASDController>();
        GameManager.Instance.InputSystem.Controllers.Remove(m_Joystick);
        if (wasd != null)
            Destroy(wasd);

        // 유저가 캐릭터 조작불가능
        player.Controlable = false;

        // 카메라 회전 불가능
        GameManager.Instance.InputSystem.CameraRotatable = false;
    }

    public override void OnOpened()
    {
        var player = GameManager.Instance.Player;

        // 컨트롤러 추가
        GameManager.Instance.InputSystem.Controllers.Add(m_Joystick);
        if (Application.platform != RuntimePlatform.Android)
        {
            var wasd = player.gameObject.AddComponent<WASDController>();
            GameManager.Instance.InputSystem.Controllers.Add(wasd);
        }

        // 유저가 캐릭터 조작가능
        player.Controlable = true;

        // 카메라 회전 가능
        GameManager.Instance.InputSystem.CameraRotatable = true;

        // Canvas의 RenderMode가 Screen Space - Camera일 경우 조이스틱이 정상적으로 작동하지 않는 문제 발생
        GameManager.Instance.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SkillButtonSetup(player.CurrentCharacter);
    }

    /// <summary> 캐릭터의 따라 스킬버튼을 세팅합니다. </summary>
    public void SkillButtonSetup(Character character)
    {
        // 기본공격 세팅
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

        // 스킬 세팅
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

        // 대쉬 세팅
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
