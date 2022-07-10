using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateRun00Behavior : MonsterPirateBehavior
{
    protected override AniType m_AniType => AniType.RUN_0;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Pirate.Agent.pathEndPosition == m_Pirate.transform.position)
        {
            // Ÿ���� �����ؾ��ϴ� ���
            if (m_Pirate.Target != null)
            {
                // SetAttackBehavior()�� �� ���̻� ȣ��Ǵ� ���� �����ϱ� ���� ���ǹ�
                if (m_Pirate.Decision != MonsterPirateDecision.Attack)
                    m_Pirate.SetAttackBehavior();
            }
        }
            
    }
}
