using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSpeedBoostEffect : Effect
{
    public override ObjectCode Code => ObjectCode.EFFECT_KnightSpeedBoost;

    protected override void OnPoolLoaded()
    {
        gameObject.SetActive(true);
        StartCoroutine(AutoRelease());
    }

    protected override void OnPoolReleased()
    {
        gameObject.SetActive(false);
    }
}
