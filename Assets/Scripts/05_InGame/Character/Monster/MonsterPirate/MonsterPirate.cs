using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class MonsterPirate : Monster
{
    public override ObjectCode Code => ObjectCode.CHAR_MonsterPirate;

    public void ShootBullet()
    {
        AnimationJobs.Enqueue(AniType.ATTACK_0);
    }

    // -----------------------------------------------------------------------

    protected override void OnGoingTo(Vector3 position)
    {
        AnimationJobs.Enqueue(AniType.RUN_0);
    }
}