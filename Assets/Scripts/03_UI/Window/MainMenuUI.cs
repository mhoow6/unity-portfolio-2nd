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

    bool m_Init;
    Vector3 m_OriginNickNameAnchoredPosition;

    // �� ���� ������ ��� LevelNickName�� rectTransform�� �����ؾ���.
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

        // ĳ���� Ŭ���� �ִϸ��̼� �߻� ����
        if (MainMenuMechanism.Instance != null)
            MainMenuMechanism.Instance.CheckUserClickingTheCharacter = false;
    }

    public override void OnOpened()
    {
        var playerData = GameManager.PlayerData;
        // ���� ����ÿ� �� �͵�
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

        // ���� ī�޶� Ű��
        GameManager.Instance.MainCam.gameObject.SetActive(true);

        // ������ �г���
        LevelNickName.text = $"Lv.{playerData.Level} <size=50>{playerData.NickName}</size>";
        // Ʈ���� ȿ��
        LevelNickName.rectTransform.anchoredPosition = m_OriginNickNameAnchoredPosition;
        LevelNickName.rectTransform.DOAnchorPosX(LevelNickName.rectTransform.anchoredPosition.x + LEVELNICKNAME_TWEEN_DELTA, TWEEN_DURATION);

        // ����ġ �����̴�
        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(info => info.Level == playerData.Level).MaxExperience;
        ExperienceSlider.value = 0;
        ExperienceSlider.maxValue = maxExperience;
        ExperienceSlider.DOValue(playerData.Experience, TWEEN_DURATION);

        // ������, ��
        StatusDisplay.SetData();

        // ĳ���� Ŭ���� �ִϸ��̼� �߻� ����
        if (MainMenuMechanism.Instance != null)
            MainMenuMechanism.Instance.CheckUserClickingTheCharacter = true;
    }
}
