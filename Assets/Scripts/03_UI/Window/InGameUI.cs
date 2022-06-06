using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.UI;

public class InGameUI : UI
{
    public override UIType Type => UIType.InGame;
    public List<CharacterButtonDisplay> CharacterButtonDisplays = new List<CharacterButtonDisplay>();

    [SerializeField] VirtualJoystick m_Joystick;

    [Space(10)]
    [SerializeField] SkillButtonDisplay m_AttackButton;
    [SerializeField] SkillButtonDisplay m_DashButton;
    [SerializeField] SkillButtonDisplay m_SkillButton;

    [Space(10)]
    [SerializeField] CustomSlider m_HpSlider;
    [SerializeField] Text m_HpText;

    [SerializeField] CustomSlider m_SpSlider;
    [SerializeField] Text m_SpText;

    [SerializeField] CustomSlider m_TargetSlider;
    [SerializeField] Text m_TargetNameText;

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

        // 이벤트 해제
        foreach (var character in player.Characters)
            character.DisposeEvents();
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

    /// <summary> 캐릭터의 따라 HP,SP 슬라이더를 세팅합니다. </summary>
    public void SettingSliders(Character character)
    {
        m_HpSlider.SetData(
            0,
            character.MaxHp,
            character.Hp,
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
            character.Sp,
            onValueUpdate: (sp) =>
            {
                m_SpText.text = $"{sp} / {character.MaxSp}";
            });
        m_SpText.text = $"{character.Sp} / {character.MaxSp}";
        character.OnSpUpdate +=
            (sp) =>
            {
                m_HpSlider.Value = sp;
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
            target.MaxHp,
            target.Hp,
            1);

        // 타겟의 Hp에 따라 슬라이더 값 변경
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
}
