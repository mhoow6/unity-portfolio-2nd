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
    public SplitSlider ExperienceSlider;
    public StatusUI StatusDisplay;

    [SerializeField] RectTransform m_AdventureBtnRectTransform;
    [SerializeField] RectTransform m_EquipmentBtnRectTransform;
    [SerializeField] RectTransform m_CharacterBtnRectTransform;

    bool m_Init;
    Vector3 m_OriginNickNameAnchoredPosition;

    // �� ���� ������ ��� LevelNickName�� rectTransform�� �����ؾ���.
    const int LEVELNICKNAME_TWEEN_DELTA = 50;
    const float EXPERIENCE_VALUE_TWEEN_DURATION = 2.0f;

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
        StopAllCoroutines();
        var lobbyManager = LobbyManager.Instance;
        if (lobbyManager)
        {
            lobbyManager.MainCam.gameObject.SetActive(false);

            // ĳ���� Ŭ���� �ִϸ��̼� �߻� ����
            lobbyManager.MainLobbySystem.CheckUserClickingTheCharacter = false;
        }
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
        LobbyManager.Instance.MainCam.gameObject.SetActive(true);

        // ������ �г���
        LevelNickName.text = $"Lv.{playerData.Level} <size=50>{playerData.NickName}</size>";

        // ����ġ �����̴�
        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(info => info.Level == playerData.Level).MaxExperience;
        ExperienceSlider.SetData(0, maxExperience);
        ExperienceSlider.Value = 0;
        StartCoroutine(ExperienceSliderDOValueCoroutine(playerData.Experience));

        // ������, ��
        StatusDisplay.SetData();

        // ĳ���� Ŭ���� �ִϸ��̼� �߻� ����
        LobbyManager.Instance.MainLobbySystem.CheckUserClickingTheCharacter = true;
    }

    IEnumerator ExperienceSliderDOValueCoroutine(float endValue)
    {
        float timer = 0f;
        while (timer < EXPERIENCE_VALUE_TWEEN_DURATION)
        {
            timer += Time.deltaTime;
            ExperienceSlider.Value = Mathf.Lerp(ExperienceSlider.Value, endValue, timer / EXPERIENCE_VALUE_TWEEN_DURATION);

            yield return null;
        }
        ExperienceSlider.Value = endValue;
    }
}
