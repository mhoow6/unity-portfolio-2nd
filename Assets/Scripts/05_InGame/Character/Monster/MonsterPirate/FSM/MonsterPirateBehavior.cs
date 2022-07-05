using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class MonsterPirateBehavior : AnimationBehaviour
{
    protected MonsterPirate m_Self;
    protected MonsterPirateBehaviorData m_BehaviorData;
    protected MonsterPirateAttackData m_AttackData;
    protected float m_DecisionTimer;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_Self == null)
        {
            m_Self = animator.GetComponent<MonsterPirate>();
            m_BehaviorData = m_Self.BehaviorData as MonsterPirateBehaviorData;
            m_AttackData = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Self.Code)] as MonsterPirateAttackData;
        }

        m_DecisionTimer = 0f;
    }
}