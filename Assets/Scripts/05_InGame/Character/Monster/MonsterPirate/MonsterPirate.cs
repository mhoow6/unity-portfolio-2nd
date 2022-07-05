using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class MonsterPirate : Monster
{
    public override ObjectCode Code => ObjectCode.CHAR_MonsterPirate;
    public MonsterPirateDecision Decision;
    public Transform BulletFiredPosition;

    public override void Attack()
    {
        AnimationJobs.Enqueue(AniType.ATTACK_0);
        Decision = MonsterPirateDecision.None;
    }

    public void ShootBullet()
    {
        // ¹ß»ç ÀÌÆåÆ®¸¦ ºÒ·¯¿Â´Ù.
        var effectPath = GameManager.GameDevelopSettings.EffectResourcePath;
        var projPath = GameManager.GameDevelopSettings.ProjectileResourcePath;
        var effect = StageManager.Instance.PoolSystem.Load<MonsterPirateBulletFiredEffect>($"{effectPath}/MonsterPirate_BulletFired");
        effect.transform.SetPositionAndRotation(BulletFiredPosition.position, BulletFiredPosition.rotation);

        // ÅºÀ» Æ÷¹°¼±ÀÇ ±Ëµµ·Î ½ð´Ù.
        var bullet = StageManager.Instance.PoolSystem.Load<MonsterPirateBullet>($"{projPath}/MonsterPirate_Bullet");
        bullet.transform.SetPositionAndRotation(BulletFiredPosition.position, BulletFiredPosition.rotation);

        var attackData = GetSkillData(GetAttackIndex(ObjectCode.CHAR_MonsterPirate)) as MonsterPirateAttackData;
        bullet.SetData(this, attackData.DamageScale);
        bullet.Shoot(transform.forward + Vector3.up, TrajectoryType.Parabola, attackData.BulletSpeed, attackData.BulletLifeTime);
    }

    // -----------------------------------------------------------------------

    protected override void OnGoingTo(Vector3 position)
    {
        AnimationJobs.Enqueue(AniType.RUN_0);
        Decision = MonsterPirateDecision.None;
    }
}

public enum MonsterPirateDecision
{
    Idle,
    Walk,
    Attack,
    None = -1
}