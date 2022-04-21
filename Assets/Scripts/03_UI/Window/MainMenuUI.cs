using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenuUI : UI
{
    public Text LevelNickName;
    public Slider ExperienceSlider;

    public StatusDisplay StatusDisplay;

    //public Text CharacterDialog;

    PlayerData m_PlayerData;
    bool m_Init;
    Vector3 m_OriginNickNameAnchoredPosition;

    // 이 값을 수정할 경우 LevelNickName의 rectTransform도 수정해야함.
    const int LEVELNICKNAME_TWEEN_DELTA = 50;
    const float TWEEN_DURATION = 0.5f;

    public override UIType Type => UIType.MainMenu;

    public void OnAdventureBtnClick()
    {
        GameManager.Instance.UISystem.OpenWindow<AdventureUI>(UIType.Adventure);
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
        GameManager.Instance.UISystem.OpenWindow(UIType.NickNameInput);
    }

    public override void OnClosed()
    {
        GameManager.Instance.MainCam.gameObject.SetActive(false);

        StopAllCoroutines();
    }

    public override void OnOpened()
    {
        // 최초 실행시에 할 것들
        if (!m_Init)
        {
            var gm = GameManager.Instance;
            m_PlayerData = gm.PlayerData;
            m_OriginNickNameAnchoredPosition = LevelNickName.rectTransform.anchoredPosition;

            if (m_PlayerData.AskForNickName == false && !GameManager.Instance.AskForNickNameSkip)
            {
                GameManager.Instance.UISystem.OpenWindow(UIType.NickNameInput);
                m_PlayerData.AskForNickName = true;
            }

            m_Init = true;
        }

        // 메인 카메라 키기
        GameManager.Instance.MainCam.gameObject.SetActive(true);

        // 레벨과 닉네임
        LevelNickName.text = $"Lv.{m_PlayerData.Level} <size=50>{m_PlayerData.NickName}</size>";
        // 트위닝 효과
        LevelNickName.rectTransform.anchoredPosition = m_OriginNickNameAnchoredPosition;
        LevelNickName.rectTransform.DOAnchorPosX(LevelNickName.rectTransform.anchoredPosition.x + LEVELNICKNAME_TWEEN_DELTA, TWEEN_DURATION);

        // 경험치 슬라이더
        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(info => info.Level == m_PlayerData.Level).MaxExperience;
        ExperienceSlider.value = 0;
        ExperienceSlider.maxValue = maxExperience;
        ExperienceSlider.DOValue(m_PlayerData.Experience, TWEEN_DURATION);

        // 에너지, 돈
        StatusDisplay.SetData();

        // 메인메뉴에서 캐릭터 클릭 시 랜덤 애니메이션
        StartCoroutine(CheckingUserClickCharacter());
    }

    IEnumerator CheckingUserClickCharacter()
    {
        while (true)
        {
            // 왼쪽 클릭시
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                RaycastHit hitInfo;
                Ray ray = GameManager.Instance.MainCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
                {
                    var player = GameManager.Instance.Player;
                    var character = player.CurrentCharacter;
                    if (hitInfo.collider.gameObject.Equals(character.gameObject))
                    {
                        int random = Random.Range(0, character.AnimationsWhenUserClick.Count);
                        AniType randomAni = character.AnimationsWhenUserClick[random];

                        character.Animator.SetInteger(character.ANITYPE_HASHCODE, (int)randomAni);
                    }
                }
            }

            yield return null;
        }

    }
}
