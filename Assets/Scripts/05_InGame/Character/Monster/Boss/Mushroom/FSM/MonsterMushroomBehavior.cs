using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class MonsterMushroomBehavior : AnimationBehaviour
{
    protected MonsterMushroom m_Self;
    protected MonsterMushroomBehaviorData m_BehaviorData;
    protected MonsterMushroomAttackData m_AttackData;
    protected float m_DecisionTimer;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_Self == null)
        {
            m_Self = animator.GetComponent<MonsterMushroom>();
            m_BehaviorData = m_Self.BehaviorData as MonsterMushroomBehaviorData;
            m_AttackData = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Self.Code)] as MonsterMushroomAttackData;
        }

        m_DecisionTimer = 0f;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_Self.Hp <= 0)
            m_Self.AnimationJobs.Enqueue(AniType.DEAD_0);
    }
}
