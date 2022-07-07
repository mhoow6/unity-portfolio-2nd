using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.UI;

public class InGameUI : UI
{
    public override UIType Type => UIType.InGame;
    public List<CharacterButtonUI> CharacterButtonDisplays = new List<CharacterButtonUI>();

    public VirtualJoystick Joystick;

    [Space(10)]
    [SerializeField] SkillButtonUI m_AttackButton;
    [SerializeField] SkillButtonUI m_DashButton;
    [SerializeField] SkillButtonUI m_UltiButton;

    [Space(10)]
    [SerializeField] SplitSlider m_HpSlider;
    [SerializeField] Text m_HpText;

    [SerializeField] SplitSlider m_SpSlider;
    [SerializeField] Text m_SpText;

    [SerializeField] SplitSlider m_TargetSlider;
    [SerializeField] Text m_TargetNameText;

    public override void OnClosed()
    {
        var player = StageManager.Instance.Player;

        // 다시 Canvas의 RenderMode를 Screen Space - Camera
        GameManager.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.UISystem.Canvas.worldCamera = GameManager.UISystem.UICamera;

        // 컨트롤러 제외
        GameManager.InputSystem.Controllers.Remove(Joystick);

        // 유저가 캐릭터 조작불가능
        player.Controlable = false;

        // 카메라 회전 불가능
        GameManager.InputSystem.CameraRotatable = false;

        // 이벤트 해제
        foreach (var character in player.Characters)
            character.DisposeEvents();
    }

    public override void OnOpened()
    {
        var player = StageManager.Instance.Player;

        // 컨트롤러 추가
        GameManager.InputSystem.Controllers.Add(Joystick);

        // 유저가 캐릭터 조작가능
        player.Controlable = true;

        // 카메라 회전 가능
        GameManager.InputSystem.CameraRotatable = true;

        // Canvas의 RenderMode가 Screen Space - Camera일 경우 조이스틱이 정상적으로 작동하지 않는 문제 발생
        GameManager.UISystem.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // 스킬 버튼 세팅
        SettingSkillButtons(player.CurrentCharacter);

        // 교체 캐릭터 버튼 세팅
        CharacterButtonDisplays.ForEach(button => button.gameObject.SetActive(false));
        for (int i = 0; i < player.Characters.Count; i++)
        {
            var character = player.Characters[i];
            var currentButton = CharacterButtonDisplays[i];

            currentButton.SetData(character);
            if (character.Equals(player.CurrentCharacter))
                currentButton.gameObject.SetActive(false);
            else
                currentButton.gameObject.SetActive(true);
        }

        // 캐릭터 HP,SP 표기
        SettingSliders(player.CurrentCharacter);
        
        // 타겟 업데이트 시 HP 상단에 표기
        m_TargetSlider.gameObject.SetActive(false);
        m_TargetNameText.gameObject.SetActive(false);
        player.CurrentCharacter.OnTargetUpdate +=
            (Character target) =>
            {
                SettingTargetSlider(target);
            };

    }

    /// <summary> 캐릭터의 따라 스킬버튼을 세팅합니다. </summary>
    public void SettingSkillButtons(Character character)
    {
        int attackIndex = Character.GetAttackIndex(character.Code);

        SkillButtonParam attackButtonParam;
        attackButtonParam.OnClick = () =>
        {
            var character = StageManager.Instance.Player.CurrentCharacter;
            character.Attack(m_AttackButton);
        };
        attackButtonParam.OnExit = () =>
        {
            GameManager.InputSystem.PressAButton = false;
        };
        attackButtonParam.SkillData = Character.GetSkillData(attackIndex);
        m_AttackButton.SetData(attackButtonParam);


        int dashIndex = Character.GetDashIndex(character.Code);
        SkillButtonParam dashButtonParam;
        dashButtonParam.SkillData = Character.GetSkillData(dashIndex);
        dashButtonParam.OnClick =
            () =>
            {
                var character = StageManager.Instance.Player.CurrentCharacter;
                character.Dash(m_DashButton);
            };
        dashButtonParam.OnExit =
            () =>
            {
                GameManager.InputSystem.PressXButton = false;
            };
        m_DashButton.SetData(dashButtonParam);


        int ultiIndex = Character.GetUltimateIndex(character.Code);
        SkillButtonParam ultiButtonParam;
        ultiButtonParam.SkillData = Character.GetSkillData(ultiIndex);
        ultiButtonParam.OnClick =
            () =>
            {
                var character = StageManager.Instance.Player.CurrentCharacter;
                character.Ultimate(m_UltiButton);
            };
        ultiButtonParam.OnExit =
            () =>
            {
                GameManager.InputSystem.PressBButton = false;
            };
        m_UltiButton.SetData(ultiButtonParam);
    }

    /// <summary> 캐릭터의 따라 HP,SP 슬라이더를 세팅합니다. </summary>
    public void SettingSliders(Character character)
    {
        m_HpSlider.SetData(
            0,
            character.MaxHp,
            onValueUpdate: (hp) =>
            {
                m_HpText.text = $"{hp}/<size=36>{character.MaxHp}</size>";
            });
        m_HpText.text = $"{character.Hp}/<size=36>{character.MaxHp}</size>";
        character.OnHpUpdate +=
            (hp) =>
            {
                m_HpSlider.Value = hp;
            };


        m_SpSlider.SetData(
            0,
            character.MaxSp,
            onValueUpdate: (sp) =>
            {
                m_SpText.text = $"{sp} / {character.MaxSp}";
            });
        m_SpText.text = $"{character.Sp} / {character.MaxSp}";
        character.OnSpUpdate +=
            (sp) =>
            {
                m_SpSlider.Value = sp;
            };
    }

    void SettingTargetSlider(Character target)
    {
        var row = TableManager.Instance.CharacterTable.Find(character => character.Code == target.Code);

        m_TargetSlider.gameObject.SetActive(true);
        m_TargetNameText.gameObject.SetActive(true);
        m_TargetNameText.text = $"{target.Name}";

        // 슬라이더 세팅
        m_TargetSlider.SetData(
            0,
            target.MaxHp);
        m_TargetSlider.Value = target.Hp;

        // 타겟의 Hp에 따라 슬라이더 값 변경
        if (!target.TargetSliderHooked)
        {
            target.OnHpUpdate +=
            (hp) =>
            {
                if (hp > 0)
                    m_TargetSlider.Value = hp;

                if (hp <= 0)
                {
                    m_TargetSlider.gameObject.SetActive(false);
                    m_TargetNameText.gameObject.SetActive(false);
                    target.DisposeEvents();
                }
            };
        }
        target.TargetSliderHooked = true;
        
    }
}
