using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparcher : Character
{
    // TODO: �ӽ�
    private void Start()
    {
        OnSpawn();
    }

    protected override void OnDead()
    {
        base.OnDead();
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();

    }
}
