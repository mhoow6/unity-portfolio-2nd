using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Knight_PreloadSettings", menuName = "Preload/Knight Preload Settings", order = 2)]
public class KnightPreloadSettings : PlayablePreloadSettings
{
    public GameObject ShieldEffectPrefab;
    public GameObject SpeedBoostPrefab;

    public override void Instantitate(Playable parent)
    {
        var sm = StageManager.Instance;

        var speedBoostEffect = sm.PoolSystem.Load<KnightSpeedBoostEffect>(SpeedBoostPrefab);
        sm.PoolSystem.Release(speedBoostEffect);

        var shieldEffect = sm.PoolSystem.Load<KnightShieldEffect>(ShieldEffectPrefab);
        sm.PoolSystem.Release(shieldEffect);
    }

    public override void Instantitate(){}
}
