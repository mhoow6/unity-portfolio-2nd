using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public sealed class MonsterPirate : Monster
{
    public override ObjectCode Code => ObjectCode.CHAR_MonsterPirate;
    public MonsterPirateDecision Decision;
    public Transform BulletFiredPosition;

    public void ShootBullet()
    {
        var preload = Preloads as MonsterPiratePreloadSettings;

        // �߻��ϴ� ���� ����Ʈ
        var effect = StageManager.Instance.PoolSystem.Load<MonsterPirateBulletFiredEffect>(preload.Prefab_BulletFired);
        effect.transform.SetPositionAndRotation(BulletFiredPosition.position, BulletFiredPosition.rotation);

        // ��ź
        var bullet = StageManager.Instance.PoolSystem.Load<MonsterPirateBullet>(preload.Prefab_Bullet);
        bullet.transform.SetPositionAndRotation(BulletFiredPosition.position, transform.rotation);

        var attackData = GetSkillData(GetAInputDataIndex(ObjectCode.CHAR_MonsterPirate)) as MonsterPirateAttackData;
        bullet.SetData(this, attackData.DamageScale);

        // ������ ��������
        Vector3 parabolaStartPosition = bullet.transform.position;
        // ������ ������
        Vector3 parabolaEndMaxPosition = bullet.transform.position + (bullet.transform.forward * attackData.BulletLifeTime * attackData.BulletSpeed);
        Vector3 parabolaEndPosition = parabolaEndMaxPosition;

        // Ÿ���� ���� ���, Ÿ������ �����ؾ� �Ѵ�.
        if (Target != null)
            parabolaEndPosition = Target.transform.position;

        // ��ź�� ���� ������Ʈ ����
        Ray ray = new Ray(parabolaEndPosition, Vector3.down);
        RaycastHit hitInfo;
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            parabolaEndPosition = hitInfo.point;

        // �������� �ӵ��� �Ÿ��� ���� �����ؾ� �Ѵ�. (�ִ� BulletSpeed)
        float distanceRatio = Vector3.Distance(parabolaStartPosition, parabolaEndPosition) / Vector3.Distance(parabolaStartPosition, parabolaEndMaxPosition);
        float bulletSpeed = attackData.BulletSpeed * distanceRatio;

        // �������� ���� ���� �����ϱ� (�ִ� BulletShootHeight)
        float bulletHeight = attackData.BulletShootHeight * distanceRatio;

        // ź�� �������� �˵��� ���.
        bullet.ShootParabola(parabolaStartPosition, parabolaEndPosition, bulletHeight, bulletSpeed);
    }

    // -----------------------------------------------------------------------

    protected override void OnSetWalkBehavior(Vector3 position)
    {
        AnimationJobs.Enqueue(AniType.RUN_0);
        Decision = MonsterPirateDecision.Walk;
    }

    protected override void OnSetAttackBehavior()
    {
        AnimationJobs.Enqueue(AniType.ATTACK_0);
        Decision = MonsterPirateDecision.Attack;
    }
}

public enum MonsterPirateDecision
{
    Idle,
    Walk,
    Attack,
    None = -1
}