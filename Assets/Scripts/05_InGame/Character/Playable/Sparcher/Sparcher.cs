using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Sparcher : Playable
{
    public override ObjectCode Code => ObjectCode.CHAR_Sparcher;

    [SerializeField] Transform m_ArrowSpawnPosition;

    protected override void OnSpawn()
    {
        base.OnSpawn();

        // SkillDatas.json
        PassiveSkill = new SparcherPassiveSkill(GetPassiveIndex(Code));
    }

    public override void Attack(int skillIndex)
    {
        var origin = JsonManager.Instance.JsonDatas[skillIndex];

        if (skillIndex == GetAttackIndex(Code))
        {
            SparcherBasicAttackData data = origin as SparcherBasicAttackData;
            var config = GameManager.GameDevelopSettings;

            // ȭ�� �ν��Ͻ�
            var proj = StageManager.Instance.PoolSystem.Load<Projectile>($"{config.ProjectileResourcePath}/{data.ArrowPrefabPath}");
            proj.SetData(this, (DamageType)data.DamageType, data.DamageScale);

            // ȭ���� forward�� ȭ�������� �Ǿ����� �ʾ� ó�� �ν��Ͻ��Ҷ� ������ ȸ��
            var spawnRotation = transform.eulerAngles + new Vector3(0, 90f, 0f);
            proj.transform.SetPositionAndRotation(m_ArrowSpawnPosition.position, Quaternion.Euler(spawnRotation));

            // ȭ�� �߻�
            proj.Shoot(transform.forward, TrajectoryType.Straight, data.ArrowMoveSpeed, data.ArrowLifeTime);
        }
    }

    protected override bool CanDash()
    {
        if (AniType >= AniType.JUMP_0 && AniType <= AniType.JUMP_4)
            return false;

        if (AniType >= AniType.ATTACK_0 && AniType <= AniType.ATTACK_4 || AniType >= AniType.ATTACK_5 && AniType <= AniType.ATTACK_9)
            return false;

        if (StageManager.Instance.Player.Moveable == false)
            return false;

        return true;
    }
}
