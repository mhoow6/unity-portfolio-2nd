using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainLobbyUI : UI
{
    public Text LevelNickName;
    public Slider ExperienceSlider;
    public StatusDisplay StatusDisplay;

    [SerializeField] RectTransform m_AdventureBtnRectTransform;
    [SerializeField] RectTransform m_EquipmentBtnRectTransform;
    [SerializeField] RectTransform m_CharacterBtnRectTransform;

    bool m_Init;
    Vector3 m_OriginNickNameAnchoredPosition;

    // �� ���� ������ ��� LevelNickName�� rectTransform�� �����ؾ���.
    const int LEVELNICKNAME_TWEEN_DELTA = 50;
    const float TWEEN_DURATION = 0.5f;

    public override UIType Type => UIType.MainLobby;

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
        GameManager.MainCam.gameObject.SetActive(false);

        // ĳ���� Ŭ���� �ִϸ��̼� �߻� ����
        if (MainMenuSystem.Instance != null)
            MainMenuSystem.Instance.CheckUserClickingTheCharacter = false;
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

        // ��ư ������ �� ����
        m_AdventureBtnRectTransform.localScale = Vector3.one;
        m_EquipmentBtnRectTransform.localScale = Vector3.one;
        m_CharacterBtnRectTransform.localScale = Vector3.one;

        // ���� ī�޶� Ű��
        GameManager.MainCam.gameObject.SetActive(true);

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
        if (MainMenuSystem.Instance != null)
            MainMenuSystem.Instance.CheckUserClickingTheCharacter = true;
    }
}
