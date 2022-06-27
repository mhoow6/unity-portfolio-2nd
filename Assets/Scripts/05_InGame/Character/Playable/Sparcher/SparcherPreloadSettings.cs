using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sparcher_PreloadSettings", menuName = "Sparcher Preload Settings", order = 2)]
public class SparcherPreloadSettings : PreloadSettings
{
    public GameObject UltimateCutscene { get; private set; }
    public GameObject UltimateEffect { get; private set; }

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
        
        var cutscene = sm.PoolSystem.Load<PoolableObject>(Prefab_UltimateCutscene, sparcher.transform);
        UltimateCutscene = cutscene.gameObject;

        var effect = sm.PoolSystem.Load<PoolableObject>(Prefab_UltimateSkillEffect);
        sm.PoolSystem.Release(effect);
        UltimateEffect = effect.gameObject;
    }
}
