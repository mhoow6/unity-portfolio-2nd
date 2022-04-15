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

    // �� ���� ������ ��� LevelNickName�� rectTransform�� �����ؾ���.
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
        m_PlayerData.OnNickNameUpdate -= (nickname) => { LevelNicknameUpdate(nickname); };

        StopAllCoroutines();
    }

    public override void OnOpened()
    {
        // ���� �����
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

        // �̺�Ʈ
        m_PlayerData.OnNickNameUpdate += (nickname) => { LevelNicknameUpdate(nickname); };

        // UI
        LevelNicknameUpdate(m_PlayerData.NickName);

        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(info => info.Level == m_PlayerData.Level).MaxExperience;
        ExperienceSlider.maxValue = maxExperience;

        StatusDisplay.SetData();

        //CharacterDialog.gameObject.SetActive(false);

        // Ʈ����
        LevelNickName.rectTransform.anchoredPosition = m_OriginNickNameAnchoredPosition;
        LevelNickName.rectTransform.DOAnchorPosX(LevelNickName.rectTransform.anchoredPosition.x + LEVELNICKNAME_TWEEN_DELTA, TWEEN_DURATION);

        ExperienceSlider.value = 0;
        ExperienceSlider.DOValue(m_PlayerData.Experience, TWEEN_DURATION);

        // ���θ޴����� ĳ���� Ŭ�� �� ���� �ִϸ��̼�
        StartCoroutine(CheckingUserClickCharacter());
    }

    void LevelNicknameUpdate(string nickname)
    {
        LevelNickName.text = $"Lv.{m_PlayerData.Level} <size=50>{nickname}</size>";
    }

    IEnumerator CheckingUserClickCharacter()
    {
        while (true)
        {
            // ���� Ŭ����
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
