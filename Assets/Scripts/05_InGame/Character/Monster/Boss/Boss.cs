using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : Monster
{
    protected override void OnDead(Character attacker, int damage)
    {
        base.OnDead(attacker, damage);

        var manager = StageManager.Instance;

        // ų ī��Ʈ
        manager.StageResult.BossKillCount++;
    }
}
