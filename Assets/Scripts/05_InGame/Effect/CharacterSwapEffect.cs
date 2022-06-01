using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwapEffect : Effect
{
    public override ObjectCode Code => ObjectCode.EFFECT_CharacterSwap;

    protected override void OnPoolLoaded()
    {
        StartCoroutine(AutoRelease());
    }
}
