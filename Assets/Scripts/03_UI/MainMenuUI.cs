using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using DG.Tweening;

public class MainMenuUI : UI
{
    public Text LevelNickName;
    public Slider ExperienceSlider;
    public Text Energy;
    public Text Gold;

    PlayerData m_PlayerData;
    bool m_Init;
    Vector3 m_OriginNickNameAnchoredPosition;

    public override UIType Type => UIType.MainMenu;

    public void OnAdventureBtnClick()
    {
        
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

    public void OnTestBtnClick()
    {
        m_PlayerData.Save();
    }

    public override void OnClosed()
    {
        LevelNickName.rectTransform.anchoredPosition = m_OriginNickNameAnchoredPosition;
        m_PlayerData.OnEnergyUpdate -= EnergyTextUpdate;
    }

    public override void OnOpened()
    {
        // 최초 실행시
        if (!m_Init)
        {
            var gm = GameManager.Instance;
            m_PlayerData = gm.PlayerData;
            m_OriginNickNameAnchoredPosition = LevelNickName.rectTransform.anchoredPosition;

            m_Init = true;
        }
        // ---

        // 이벤트 등록 
        m_PlayerData.OnEnergyUpdate += EnergyTextUpdate;
        // ---

        // UI
        LevelNickName.text = $"Lv.{m_PlayerData.Level} <size=50>{m_PlayerData.NickName}</size>";

        int maxExperience = TableManager.Instance.PlayerLevelExperienceTables.Find(info => info.Level == m_PlayerData.Level).MaxExperience;
        ExperienceSlider.maxValue = maxExperience;
        //ExperienceSlider.value = m_PlayerData.Experience;

        Gold.text = $"{m_PlayerData.Gold}";

        EnergyTextUpdate(m_PlayerData.Energy);

        // ---

        // 트위닝
        LevelNickName.rectTransform.DOAnchorPosX(LevelNickName.rectTransform.anchoredPosition.x + 50, 0.5f);
        ExperienceSlider.DOValue(m_PlayerData.Experience, 0.5f);
    }

    void EnergyTextUpdate(int energy)
    {
        int maxEnergy = TableManager.Instance.PlayerLevelEnergyTables.Find(info => info.Level == m_PlayerData.Level).MaxEnergy;
        Energy.text = $"{energy}/{maxEnergy}";
    }
}
