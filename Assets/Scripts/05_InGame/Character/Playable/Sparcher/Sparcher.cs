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

        // 화살 인스턴싱
        var proj = StageManager.Instance.PoolSystem.Load<Projectile>($"{config.ProjectileResourcePath}/{data.ArrowPrefabPath}");
        proj.SetData(this, data.DamageScale);

        // 화살의 forward가 화살촉으로 되어있지 않아 처음 인스턴싱할때 강제로 회전
        var spawnRotation = transform.eulerAngles + new Vector3(0, 90f, 0f);
        proj.transform.SetPositionAndRotation(m_ArrowSpawnPosition.position, Quaternion.Euler(spawnRotation));

        // 화살 발사
        proj.ShootStraight(transform.forward, data.ArrowMoveSpeed, data.ArrowLifeTime);
    }

    public override bool CanXInput()
    {
        // 애니메이션이 전환중일 경우 정상동작이 안 됨
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
        // 애니메이션이 전환중일 경우 정상동작이 안 됨
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
        // 애니메이션이 전환중일 경우 정상동작이 안 됨
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
