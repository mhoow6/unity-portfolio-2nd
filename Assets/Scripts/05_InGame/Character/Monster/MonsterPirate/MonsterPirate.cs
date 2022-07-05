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
        // �߻��ϴ� ���� ����Ʈ
        var effectPath = GameManager.GameDevelopSettings.EffectResourcePath;
        var projPath = GameManager.GameDevelopSettings.ProjectileResourcePath;
        var effect = StageManager.Instance.PoolSystem.Load<MonsterPirateBulletFiredEffect>($"{effectPath}/MonsterPirate_BulletFired");
        effect.transform.SetPositionAndRotation(BulletFiredPosition.position, BulletFiredPosition.rotation);

        // ��ź
        var bullet = StageManager.Instance.PoolSystem.Load<MonsterPirateBullet>($"{projPath}/MonsterPirate_Bullet");
        bullet.transform.SetPositionAndRotation(BulletFiredPosition.position, transform.rotation);

        var attackData = GetSkillData(GetAttackIndex(ObjectCode.CHAR_MonsterPirate)) as MonsterPirateAttackData;
        bullet.SetData(this, attackData.DamageScale);

        // ������ ��������
        Vector3 parabolaStartPosition = bullet.transform.position;
        // ������ ������
        Vector3 parabolaEndPosition = bullet.transform.position + (bullet.transform.forward * attackData.BulletLifeTime * attackData.BulletSpeed);
        // ��ź�� ���� ������Ʈ ����
        Ray ray = new Ray(parabolaEndPosition, Vector3.down);
        RaycastHit hitInfo;
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            parabolaEndPosition = hitInfo.point;

        // ź�� �������� �˵��� ���.
        bullet.ShootParabola(parabolaStartPosition, parabolaEndPosition, 2f, attackData.BulletSpeed);
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