using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class MonsterPirateBehavior : CharacterBehavior
{
    protected MonsterPirate m_Pirate;
    protected MonsterPirateBehaviorData m_BehaviorData;
    protected MonsterPirateAttackData m_AttackData;
    protected float m_DecisionTimer;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        if (m_Pirate == null)
        {
            m_Pirate = animator.GetComponent<MonsterPirate>();
            m_BehaviorData = m_Pirate.BehaviorData as MonsterPirateBehaviorData;
            m_AttackData = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Pirate.Code)] as MonsterPirateAttackData;
        }

        m_DecisionTimer = 0f;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Pirate.Hp <= 0)
            m_Pirate.AnimationJobs.Enqueue(AniType.DEAD_0);
    }
}