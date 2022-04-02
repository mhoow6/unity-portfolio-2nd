using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TableSystem;
using DG.Tweening;

public class MainMenuUI : UI
{
    public Text LevelNickName;
    public Animator LevelNickNameAnimator;
    public Slider ExperienceSlider;

    RectTransform m_LevelNickNameRectTransform;
    PlayerData m_PlayerData;
    bool m_Init;

    public override UIType Type => UIType.MainMenu;

    public void OnDataSaveBtnClick()
    {
        m_PlayerData.Save();
    }

    public void OnLevelUpBtnClick()
    {
        m_PlayerData.Level++;
    }

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        var gm = GameManager.Instance;
        // 이벤트 등록
        if (!m_Init)
        {
            m_PlayerData = gm.PlayerData;
            // 레벨
            m_PlayerData.OnLevelUpdate += (level) =>
            {
                LevelNickName.text = $"Lv.{m_PlayerData.Level} <size=50>{m_PlayerData.NickName}</size>";
            };

            m_LevelNickNameRectTransform = LevelNickName.GetComponent<RectTransform>();

            m_Init = true;
        }
        // ---

        // UI
        LevelNickName.text = $"Lv.{m_PlayerData.Level} <size=50>{m_PlayerData.NickName}</size>";

        int maxExperience = TableManager.Instance.PlayerLevelExperienceTables.Find(info => info.Level == m_PlayerData.Level).MaxExperience;
        ExperienceSlider.maxValue = maxExperience;
        ExperienceSlider.value = m_PlayerData.Experience;
        // ---

        // 트위닝
        LevelNickNameAnimator.SetTrigger("OnOpened");
    }
}
