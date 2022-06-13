using System.Collections;
using System.Collections.Generic;
using DatabaseSystem;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : Display
{
    public Text Energy;
    public Image EnergyBackground;
    public GameObject EnergyMax;

    public Text Gold;

    const float ENERGY_ALPHA = 0.31f;
    const float ENERGY_MAX_ALPHA = 0.5f;

    public override void SetData()
    {
        var playerData = GameManager.PlayerData;

        // 이벤트 등록 
        playerData.OnEnergyUpdate += EnergyTextUpdate;

        Gold.text = $"{playerData.Gold}";

        EnergyTextUpdate(playerData.Energy);
    }

    private void OnDisable()
    {
        GameManager.PlayerData.DisposeEvents();
    }

    void EnergyTextUpdate(int energy)
    {
        int maxEnergy = TableManager.Instance.PlayerLevelEnergyTable.Find(info => info.Level == GameManager.PlayerData.Level).MaxEnergy;
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
