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
    public Image EnergyBackground;
    public Text EnergyMax;

    public Text Gold;

    public Text CharacterDialog;

    PlayerData m_PlayerData;
    bool m_Init;
    Vector3 m_OriginNickNameAnchoredPosition;

    const float ENERGY_ALPHA = 0.31f;
    const float ENERGY_MAX_ALPHA = 0.5f;

    // 이 값을 수정할 경우 LevelNickName의 rectTransform도 수정해야함.
    const int LEVELNICKNAME_TWEEN_DELTA = 50;
    const float TWEEN_DURATION = 0.5f;

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

            if (m_PlayerData.AskForNickName == false)
            {
                GameManager.Instance.System_UI.OpenWindow(UIType.NickNameInput);
                m_PlayerData.AskForNickName = true;
            }

            m_Init = true;
        }

        // 이벤트 등록 
        m_PlayerData.OnEnergyUpdate += EnergyTextUpdate;

        // UI
        LevelNickName.text = $"Lv.{m_PlayerData.Level} <size=50>{m_PlayerData.NickName}</size>";

        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(info => info.Level == m_PlayerData.Level).MaxExperience;
        ExperienceSlider.maxValue = maxExperience;

        Gold.text = $"{m_PlayerData.Gold}";

        EnergyTextUpdate(m_PlayerData.Energy);

        // 트위닝
        LevelNickName.rectTransform.DOAnchorPosX(LevelNickName.rectTransform.anchoredPosition.x + LEVELNICKNAME_TWEEN_DELTA, TWEEN_DURATION);
        ExperienceSlider.DOValue(m_PlayerData.Experience, TWEEN_DURATION);
    }

    void EnergyTextUpdate(int energy)
    {
        int maxEnergy = TableManager.Instance.PlayerLevelEnergyTable.Find(info => info.Level == m_PlayerData.Level).MaxEnergy;
        Energy.text = $"{energy}/{maxEnergy}";

        if (energy >= maxEnergy)
        {
            if (ColorUtility.TryParseHtmlString("#FF0000", out Color color))
                EnergyBackground.color = new Vector4(color.r, color.g, color.b, ENERGY_MAX_ALPHA);

            EnergyMax.gameObject.SetActive(true);
        }
        else
        {
            EnergyBackground.color = new Vector4(0, 0, 0, ENERGY_ALPHA);
            EnergyMax.gameObject.SetActive(false);
        }
    }
}
