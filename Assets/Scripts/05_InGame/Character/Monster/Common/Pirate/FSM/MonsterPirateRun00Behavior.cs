using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateRun00Behavior : MonsterPirateBehavior
{
    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Self.Agent.pathEndPosition == m_Self.transform.position)
        {
            // 타겟을 공격해야하는 경우
            if (m_Self.Target != null)
            {
                // SetAttackBehavior()가 두 번이상 호출되는 것을 방지하기 위해 조건문
                if (m_Self.Decision != MonsterPirateDecision.Attack)
                    m_Self.SetAttackBehavior();
            }
        }
            
    }
}
