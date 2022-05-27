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
        var currentScene = GameManager.SceneType;
        var config = GameManager.GameDevelopSettings;

        switch (currentScene)
        {
            case SceneType.MainMenu:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{config.AnimationControllerResourcePath}/Sparcher/MainMenu_Sparcher");
                break;
            default:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{config.AnimationControllerResourcePath}/Sparcher/InGame_Sparcher");
                break;
        }

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
            var proj = StageManager.PoolSystem.Load<Projectile>($"{config.ProjectileResourcePath}/{data.ArrowPrefabPath}");
            proj.SetData(this, (DamageType)data.DamageType);

            // ȭ���� forward�� ȭ�������� �Ǿ����� �ʾ� ó�� �ν��Ͻ��Ҷ� ������ ȸ��
            var spawnRotation = transform.eulerAngles + new Vector3(0, 90f, 0f);
            proj.transform.SetPositionAndRotation(m_ArrowSpawnPosition.position, Quaternion.Euler(spawnRotation));

            // ȭ�� �߻�
            proj.Shoot(transform.forward, TrajectoryType.Straight, data.ArrowMoveSpeed, data.ArrowLifeTime);
        }
    }

    public override AniType GetAniType(int skillIndex)
    {
        var origin = JsonManager.Instance.JsonDatas[skillIndex];

        if (skillIndex == GetAttackIndex(Code))
        {
            SparcherBasicAttackData data = origin as SparcherBasicAttackData;
            return (AniType)data.AniType;
        }
        return AniType.NONE;
    }

    public override int GetSpCost(int skillIndex)
    {
        var origin = JsonManager.Instance.JsonDatas[skillIndex];

        if (skillIndex == GetAttackIndex(Code))
        {
            SparcherBasicAttackData data = origin as SparcherBasicAttackData;
            return 0;
        }
        return -1;
    }
}
