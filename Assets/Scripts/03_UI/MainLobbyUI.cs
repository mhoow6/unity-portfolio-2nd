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

    const float EXPERIENCE_VALUE_TWEEN_DURATION = 2.0f;

    public override UIType Type => UIType.MainLobby;

    public void OnAdventureBtnClick()
    {
        GameManager.UISystem.OpenWindow<AdventureUI>(UIType.Adventure);
    }

    public void OnEquipmentBtnClick()
    {
        var ui = GameManager.UISystem.OpenWindow<CharacterUI>(UIType.Character);
        ui.SetData(GameManager.PlayerData.MainMenuCharacter);
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

            // 캐릭터 클릭시 애니메이션 발생 시작
            lobbyManager.MainLobbySystem.CheckUserClickingTheCharacter = false;
        }
    }

    public override void OnOpened()
    {
        var playerData = GameManager.PlayerData;
        // 최초 실행시에 할 것들
        if (playerData.AskForNickName == false && !GameManager.Instance.AskForNickNameSkip)
        {
            GameManager.UISystem.OpenWindow(UIType.NickNameInput);
            playerData.AskForNickName = true;
            return;
        }

        // 버튼 스케일 값 복구
        m_AdventureBtnRectTransform.localScale = Vector3.one;
        m_EquipmentBtnRectTransform.localScale = Vector3.one;
        m_CharacterBtnRectTransform.localScale = Vector3.one;

        // 메인 카메라 키기
        var lobbyManager = LobbyManager.Instance;
        lobbyManager.MainCam.transform.SetPositionAndRotation
            (lobbyManager.MainLobbySystem.LobbyUICameraPosition.transform.position,
            lobbyManager.MainLobbySystem.LobbyUICameraPosition.transform.rotation);

        lobbyManager.MainCam.gameObject.SetActive(true);

        // 레벨과 닉네임
        LevelNickName.text = $"Lv.{playerData.Level} <size=50>{playerData.NickName}</size>";

        // 경험치 슬라이더
        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(info => info.Level == playerData.Level).MaxExperience;
        ExperienceSlider.SetData(0, maxExperience);
        ExperienceSlider.Value = 0;
        StartCoroutine(ExperienceSliderDOValueCoroutine(playerData.Experience));

        // 캐릭터 클릭시 애니메이션 발생 시작
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
