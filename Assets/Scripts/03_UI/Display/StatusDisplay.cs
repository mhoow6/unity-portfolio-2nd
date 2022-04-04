using System.Collections;
using System.Collections.Generic;
using TableSystem;
using UnityEngine;
using UnityEngine.UI;

public class StatusDisplay : Display
{
    public Text Energy;
    public Image EnergyBackground;
    public Text EnergyMax;

    public Text Gold;

    PlayerData m_PlayerData;
    const float ENERGY_ALPHA = 0.31f;
    const float ENERGY_MAX_ALPHA = 0.5f;

    public override void SetData()
    {
        m_PlayerData = GameManager.Instance.PlayerData;

        // 이벤트 등록 
        m_PlayerData.OnEnergyUpdate += EnergyTextUpdate;

        Gold.text = $"{m_PlayerData.Gold}";

        EnergyTextUpdate(m_PlayerData.Energy);
    }

    private void OnDisable()
    {
        m_PlayerData.OnEnergyUpdate -= EnergyTextUpdate;
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
