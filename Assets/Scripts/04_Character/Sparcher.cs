using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Sparcher : Character
{
    public override ObjectCode Code => ObjectCode.CHAR_Sparcher;

    [SerializeField] Transform m_ArrowSpawnPosition;

    public readonly int ATTACK_INDEX = m_ATTACK_INDEX;
    const int m_ATTACK_INDEX = 2000;
    const int m_SKILL_INDEX = -1;
    const int m_DASH_INDEX = -1;

    protected override void OnSpawn()
    {
        var currentScene = GameManager.Instance.SceneType;
        switch (currentScene)
        {
            case SceneType.MainMenu:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("09_AnimationController/Sparcher/MainMenu_Sparcher");
                break;
            default:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("09_AnimationController/Sparcher/InGame_Sparcher");
                break;
        }

        // 스킬타입에 따른 인덱스 정해주기 (SkillDatas.json)
        SkillIndices.Add(SkillType.Attack, ATTACK_INDEX);
    }

    protected override void OnLive()
    {
        CurrentAniType = (AniType)Animator.GetInteger(ANITYPE_HASHCODE);
    }

    public override void Attack(int skillIndex)
    {
        var origin = JsonManager.Instance.JsonDatas[skillIndex];
        switch (skillIndex)
        {
            case m_ATTACK_INDEX:
                SparcherBasicAttackData data = origin as SparcherBasicAttackData;

                // 화살 인스턴싱
                var proj = StageManager.Instance.Pool.Load<Projectile>($"07_Projectile/{data.ArrowPrefabPath}");
                proj.SetData(this);
                
                // 화살의 forward가 화살촉으로 되어있지 않아 처음 인스턴싱할때 강제로 회전
                var spawnRotation = transform.eulerAngles + new Vector3(0, 90f, 0f);
                proj.transform.SetPositionAndRotation(m_ArrowSpawnPosition.position, Quaternion.Euler(spawnRotation));

                // 화살 발사
                proj.Shoot(gameObject, transform.forward, TrajectoryType.Straight, data.ArrowMoveSpeed, data.ArrowLifeTime);
                break;
        }
    }

    public override AniType GetAniType(int skillIndex)
    {
        var origin = JsonManager.Instance.JsonDatas[skillIndex];
        switch (skillIndex)
        {
            case m_ATTACK_INDEX:
                SparcherBasicAttackData data = origin as SparcherBasicAttackData;
                return (AniType)data.AniType;
            default:
                return AniType.NONE;
        }
    }

    public override int GetSpCost(int skillIndex)
    {
        var origin = JsonManager.Instance.JsonDatas[skillIndex];
        switch (skillIndex)
        {
            case m_SKILL_INDEX:
                return 0;
            default:
                return 0;
        }
    }
}
