using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sparcher_PreloadSettings", menuName = "Preload/Sparcher Preload Settings", order = 2)]
public class SparcherPreloadSettings : PreloadSettings
{
    [HideInInspector] public GameObject UltimateCutscene;
    [HideInInspector] public GameObject UltimateEffect;

    [SerializeField] GameObject Prefab_UltimateCutscene;
    [SerializeField] GameObject Prefab_UltimateSkillEffect;

    public override void Instantitate()
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        var sparcher = sm.Player.CurrentCharacter as Sparcher;

        UltimateCutscene = Instantiate(Prefab_UltimateCutscene, sparcher.transform);

        if (sm.PreloadZone)
        {
            sm.ReservingPreload(new PreloadParam()
            {
                PreloadPrefab = Prefab_UltimateSkillEffect,
                OnProcessCompletedCallback = (gameObject) =>
                {
                    UltimateEffect = gameObject;
                }
            });
        }
    }

}
