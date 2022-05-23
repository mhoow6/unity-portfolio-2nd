using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenuUI : UI
{
    public Text LevelNickName;
    public Slider ExperienceSlider;
    public StatusDisplay StatusDisplay;

    [SerializeField] RectTransform m_AdventureBtnRectTransform;
    [SerializeField] RectTransform m_EquipmentBtnRectTransform;
    [SerializeField] RectTransform m_CharacterBtnRectTransform;

    bool m_Init;
    Vector3 m_OriginNickNameAnchoredPosition;

    // 이 값을 수정할 경우 LevelNickName의 rectTransform도 수정해야함.
    const int LEVELNICKNAME_TWEEN_DELTA = 50;
    const float TWEEN_DURATION = 0.5f;

    public override UIType Type => UIType.MainMenu;

    public void OnAdventureBtnClick()
    {
        GameManager.UISystem.OpenWindow<AdventureUI>(UIType.Adventure);
    }

    public void OnEquipmentBtnClick()
    {
        
    }

    public void OnShopBtnClick()
    {
        
    }

    public void OnQuestBtnClick()
    {

    }

    public void OnSettingBtnClick()
    {

    }

    public void OnCharacterBtnClick()
    {

    }

    public void OnNickNameBtnClick()
    {
        GameManager.UISystem.OpenWindow(UIType.NickNameInput, false);
    }

    public override void OnClosed()
    {
        GameManager.Instance.MainCam.gameObject.SetActive(false);

        // 캐릭터 클릭시 애니메이션 발생 시작
        if (MainMenuMechanism.Instance != null)
            MainMenuMechanism.Instance.CheckUserClickingTheCharacter = false;
    }

    public override void OnOpened()
    {
        var playerData = GameManager.PlayerData;
        // 최초 실행시에 할 것들
        if (!m_Init)
        {
            m_OriginNickNameAnchoredPosition = LevelNickName.rectTransform.anchoredPosition;

            if (playerData.AskForNickName == false && !GameManager.Instance.AskForNickNameSkip)
            {
                GameManager.UISystem.OpenWindow(UIType.NickNameInput);
                playerData.AskForNickName = true;
            }

            m_Init = true;
        }

        // 버튼 스케일 값 복구
        m_AdventureBtnRectTransform.localScale = Vector3.one;
        m_EquipmentBtnRectTransform.localScale = Vector3.one;
        m_CharacterBtnRectTransform.localScale = Vector3.one;

        // 메인 카메라 키기
        GameManager.Instance.MainCam.gameObject.SetActive(true);

        // 레벨과 닉네임
        LevelNickName.text = $"Lv.{playerData.Level} <size=50>{playerData.NickName}</size>";
        // 트위닝 효과
        LevelNickName.rectTransform.anchoredPosition = m_OriginNickNameAnchoredPosition;
        LevelNickName.rectTransform.DOAnchorPosX(LevelNickName.rectTransform.anchoredPosition.x + LEVELNICKNAME_TWEEN_DELTA, TWEEN_DURATION);

        // 경험치 슬라이더
        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(info => info.Level == playerData.Level).MaxExperience;
        ExperienceSlider.value = 0;
        ExperienceSlider.maxValue = maxExperience;
        ExperienceSlider.DOValue(playerData.Experience, TWEEN_DURATION);

        // 에너지, 돈
        StatusDisplay.SetData();

        // 캐릭터 클릭시 애니메이션 발생 시작
        if (MainMenuMechanism.Instance != null)
            MainMenuMechanism.Instance.CheckUserClickingTheCharacter = true;
    }
}
