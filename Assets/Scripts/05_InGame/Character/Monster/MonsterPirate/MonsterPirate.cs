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
        // 발사하는 순간 이펙트
        var effectPath = GameManager.GameDevelopSettings.EffectResourcePath;
        var projPath = GameManager.GameDevelopSettings.ProjectileResourcePath;
        var effect = StageManager.Instance.PoolSystem.Load<MonsterPirateBulletFiredEffect>($"{effectPath}/MonsterPirate_BulletFired");
        effect.transform.SetPositionAndRotation(BulletFiredPosition.position, BulletFiredPosition.rotation);

        // 포탄
        var bullet = StageManager.Instance.PoolSystem.Load<MonsterPirateBullet>($"{projPath}/MonsterPirate_Bullet");
        bullet.transform.SetPositionAndRotation(BulletFiredPosition.position, transform.rotation);

        var attackData = GetSkillData(GetAttackIndex(ObjectCode.CHAR_MonsterPirate)) as MonsterPirateAttackData;
        bullet.SetData(this, attackData.DamageScale);

        // 포물선 시작지점
        Vector3 parabolaStartPosition = bullet.transform.position;
        // 포물선 끝지점
        Vector3 parabolaEndMaxPosition = bullet.transform.position + (bullet.transform.forward * attackData.BulletLifeTime * attackData.BulletSpeed);
        Vector3 parabolaEndPosition = parabolaEndMaxPosition;

        // 타겟이 있을 경우, 타겟한테 조준해야 한다.
        if (Target != null)
            parabolaEndPosition = Target.transform.position;

        // 포탄이 닿을 오브젝트 감지
        Ray ray = new Ray(parabolaEndPosition, Vector3.down);
        RaycastHit hitInfo;
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            parabolaEndPosition = hitInfo.point;

        // 포물선의 속도는 거리에 따라 조절해야 한다. (최대 BulletSpeed)
        float distanceRatio = Vector3.Distance(parabolaStartPosition, parabolaEndPosition) / Vector3.Distance(parabolaStartPosition, parabolaEndMaxPosition);
        float bulletSpeed = attackData.BulletSpeed * distanceRatio;

        // 포물선의 높이 또한 조절하기 (최대 BulletShootHeight)
        float bulletHeight = attackData.BulletShootHeight * distanceRatio;

        // 탄을 포물선의 궤도로 쏜다.
        bullet.ShootParabola(parabolaStartPosition, parabolaEndPosition, bulletHeight, bulletSpeed);
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