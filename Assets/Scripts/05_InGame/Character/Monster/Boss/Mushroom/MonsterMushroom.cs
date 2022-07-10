using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public sealed class MonsterMushroom : Boss
{
    public override ObjectCode Code => ObjectCode.CHAR_MonsterMushroom;
    public MonsterMushroomDecision Decision;

    #region AI
    public void SetAttack02Behavior()
    {
        AnimationJobs.Enqueue(AniType.ATTACK_1);
        Decision = MonsterMushroomDecision.Attack02;
    }

    public void SetAttack03Behavior()
    {
        AnimationJobs.Enqueue(AniType.ATTACK_2);
        Decision = MonsterMushroomDecision.Attack03;
    }

    public void SetDizzyBehavior()
    {
        AnimationJobs.Enqueue(AniType.SIT_0);
        Decision = MonsterMushroomDecision.Sit;
    }

    public void SetRiseBehavior()
    {
        AnimationJobs.Enqueue(AniType.RISE_0);
        Decision = MonsterMushroomDecision.Rise;
    }
    #endregion

    // -----------------------------------------------------------------------

    #region AI
    protected override void OnSetWalkBehavior(Vector3 position)
    {
        AnimationJobs.Enqueue(AniType.RUN_0);
        Decision = MonsterMushroomDecision.Walk;
    }

    protected override void OnSetIdleBehavior()
    {
        AnimationJobs.Enqueue(AniType.IDLE_0);
        Decision = MonsterMushroomDecision.Idle;
    }

    protected override void OnSetAttackBehavior()
    {
        AnimationJobs.Enqueue(AniType.ATTACK_0);
        Decision = MonsterMushroomDecision.Attack01;
    }
    #endregion
}

public enum MonsterMushroomDecision
{
    Idle,
    Walk,
    Attack01,
    Attack02,
    Attack03,
    Sit,
    Rise,
    None = -1
}