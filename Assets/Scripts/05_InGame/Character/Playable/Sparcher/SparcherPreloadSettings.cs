using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sparcher_PreloadSettings", menuName = "Preload/Sparcher Preload Settings", order = 2)]
public class SparcherPreloadSettings : PlayablePreloadSettings
{
    [HideInInspector] public GameObject UltimateEffect;

    [SerializeField] GameObject Prefab_UltimateCutscene;
    [SerializeField] GameObject Prefab_UltimateSkillEffect;

    public override void Instantitate()
    {
        
    }

    public override void Instantitate(Playable parent)
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        Instantiate(Prefab_UltimateCutscene, parent.transform);

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
