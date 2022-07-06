using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateBulletFiredEffect : Effect
{
    protected override void OnPoolLoaded()
    {
        if (gameObject.activeSelf)
            StartCoroutine(AutoRelease());
    }
}
