using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Sparcher : Playable
{
    public override ObjectCode Code => ObjectCode.CHAR_Sparcher;

    public void ShootArrow()
    {
        var origin = JsonManager.Instance.JsonDatas[GetAInputDataIndex(Code)];
        SparcherBasicAttackData data = origin as SparcherBasicAttackData;
        var config = GameManager.GameDevelopSettings;

        // ȭ�� �ν��Ͻ�
        var proj = StageManager.Instance.PoolSystem.Load<Projectile>($"{config.ProjectileResourcePath}/{data.ArrowPrefabPath}");
        proj.SetData(this, data.DamageScale);

        // ȭ���� forward�� ȭ�������� �Ǿ����� �ʾ� ó�� �ν��Ͻ��Ҷ� ������ ȸ��
        var spawnRotation = transform.eulerAngles + new Vector3(0, 90f, 0f);
        proj.transform.SetPositionAndRotation(m_ArrowSpawnPosition.position, Quaternion.Euler(spawnRotation));

        // ȭ�� �߻�
        proj.ShootStraight(transform.forward, data.ArrowMoveSpeed, data.ArrowLifeTime);
    }

    public override bool CanXInput()
    {
        // �ִϸ��̼��� ��ȯ���� ��� �������� �� ��
        if (Animator.IsInTransition(AnimatorBaseLayerIndex))
            return false;

        var player = StageManager.Instance.Player;
        AniType currentAni = AniType;

        if (currentAni >= AniType.JUMP_0 && currentAni <= AniType.JUMP_4)
            return false;

        if (currentAni >= AniType.ATTACK_0 && currentAni <= AniType.ATTACK_4 || currentAni >= AniType.ATTACK_5 && currentAni <= AniType.ATTACK_9)
            return false;

        if (currentAni >= AniType.DASH_0 && currentAni <= AniType.DASH_4)
            return false;

        if (player.Moveable == false)
            return false;

        return true;
    }

    public override bool CanAInput()
    {
        // �ִϸ��̼��� ��ȯ���� ��� �������� �� ��
        if (Animator.IsInTransition(AnimatorBaseLayerIndex))
            return false;

        AniType currentAni = AniType;
        if (currentAni >= AniType.JUMP_0 && currentAni <= AniType.JUMP_4)
            return false;

        if (currentAni >= AniType.ATTACK_0 && currentAni <= AniType.ATTACK_4 || currentAni >= AniType.ATTACK_5 && currentAni <= AniType.ATTACK_9)
            return false;

        return true;
    }

    public override bool CanBInput()
    {
        // �ִϸ��̼��� ��ȯ���� ��� �������� �� ��
        if (Animator.IsInTransition(AnimatorBaseLayerIndex))
            return false;

        var skillData = GetSkillData(GetBInputDataIndex(Code));
        AniType currentAni = AniType;

        if (currentAni >= AniType.JUMP_0 && currentAni <= AniType.JUMP_4)
            return false;

        if (currentAni >= AniType.ATTACK_0 && currentAni <= AniType.ATTACK_4 || currentAni >= AniType.ATTACK_5 && currentAni <= AniType.ATTACK_9)
            return false;

        if (Sp < skillData.SpCost)
            return false;

        if (BCoolTime > 0f)
            return false;

        return true;
    }

    // -----------------------------------------------------------------------

    protected override void OnSpawn()
    {
        base.OnSpawn();

        // SkillDatas.json
        PassiveSkill = new SparcherPassiveSkill(GetPassiveIndex(Code));
    }
    // -----------------------------------------------------------------------

    [SerializeField] Transform m_ArrowSpawnPosition;
}
