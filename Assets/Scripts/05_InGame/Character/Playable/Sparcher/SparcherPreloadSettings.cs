using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sparcher_PreloadSettings", menuName = "Sparcher Preload Settings", order = 2)]
public class SparcherPreloadSettings : PreloadSettings
{
    GameObject UltimateCutscene;
    GameObject UltimateEffect;

    [Header("수동 기입")]
    [SerializeField] GameObject Prefab_UltimateCutscene;
    [SerializeField] GameObject Prefab_UltimateSkillEffect;

    bool m_Instantitated = false;

    public override void Instantitate()
    {
        if (m_Instantitated)
            return;
        m_Instantitated = true;

        var sm = StageManager.Instance;
        var sparcher = sm.Player.CurrentCharacter as Sparcher;

        UltimateCutscene = Instantiate(Prefab_UltimateCutscene, sparcher.transform);
        //UltimateEffect = Instantiate(Prefab_UltimateSkillEffect, sparcher.transform);
        //UltimateEffect.gameObject.SetActive(false);
    }
}
