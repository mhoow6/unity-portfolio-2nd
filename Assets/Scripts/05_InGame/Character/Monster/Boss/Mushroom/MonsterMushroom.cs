using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public sealed class MonsterMushroom : Boss
{
    public override ObjectCode Code => ObjectCode.CHAR_MonsterMushroom;
    public MonsterMushroomDecision Decision;
    public MonsterMushroomAttack02Hitbox Attack02Hitbox;

    #region AI
    public void SetAttack02Behavior()
    {
        // 애니메이션 변경로 인해 ATTACK_1 = Attack02
        AnimationJobs.Enqueue(AniType.ATTACK_1);
        Decision = MonsterMushroomDecision.Attack02;
    }

    public void SetAttack03Behavior()
    {
        // 애니메이션 변경로 인해 ATTACK_2 = Attack03
        AnimationJobs.Enqueue(AniType.ATTACK_2);
        Decision = MonsterMushroomDecision.Attack03;
    }

    public void SetSitBehavior()
    {
        AnimationJobs.Enqueue(AniType.SIT_0);
        Agent.SetDestination(transform.position);

        Decision = MonsterMushroomDecision.Sit;
    }

    public void SetRiseBehavior()
    {
        AnimationJobs.Enqueue(AniType.RISE_0);
        Decision = MonsterMushroomDecision.Rise;
    }
    #endregion

    public void Attack01()
    {
        // 애니메이션 퀄리티가 떨어져서 폐기
    }

    public void Attack02()
    {
        var attackdata = JsonManager.Instance.JsonDatas[GetAttackIndex(Code) + 1] as MonsterMushroomAttack02Data;

        Attack02Hitbox.ResetHitbox();
        Attack02Hitbox.gameObject.SetActive(true);
        StartCoroutine(Attack02HitboxOffCoroutine(attackdata.HitboxLifeTime));
    }

    public void Attack03()
    {
        var attackdata = JsonManager.Instance.JsonDatas[GetAttackIndex(Code) + 2] as MonsterMushroomAttack03Data;
        var param = new IsTargetInParam()
        {
            Target = Target,
            DetectAngle = attackdata.AttackAngle,
            DetectRange = attackdata.AttackDistance
        };

        if (IsTargetIn(param))
        {
            var result = CalcuateDamage(Target, attackdata.DamageScale);
            DamagedParam damageparam = new DamagedParam()
            {
                Attacker = this,
                Damage = result.Damage,
                IsCrit = result.IsCrit
            };

            Target.Damaged(damageparam);
        }
    }

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

    protected override void OnSpawn()
    {
        base.OnSpawn();

        // 히트박스 세팅
        var attackdata = JsonManager.Instance.JsonDatas[GetAttackIndex(Code) + 1] as MonsterMushroomAttack02Data;
        Attack02Hitbox.gameObject.SetActive(false);
        Attack02Hitbox.SetData(this, attackdata.MaximumHits, attackdata.DamageScale);
    }

    protected override void OnGroggied()
    {
        base.OnGroggied();

        SetSitBehavior();
    }

    protected override void OnGroggyOut()
    {
        base.OnGroggyOut();

        SetRiseBehavior();
    }

    // -----------------------------------------------------------------------

    IEnumerator Attack02HitboxOffCoroutine(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Attack02Hitbox.gameObject.SetActive(false);
    }
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