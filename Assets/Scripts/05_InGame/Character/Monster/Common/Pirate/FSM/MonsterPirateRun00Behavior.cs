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
            // Ÿ���� �����ؾ��ϴ� ���
            if (m_Self.Target != null)
            {
                // SetAttackBehavior()�� �� ���̻� ȣ��Ǵ� ���� �����ϱ� ���� ���ǹ�
                if (m_Self.Decision != MonsterPirateDecision.Attack)
                    m_Self.SetAttackBehavior();
            }
        }
            
    }
}
