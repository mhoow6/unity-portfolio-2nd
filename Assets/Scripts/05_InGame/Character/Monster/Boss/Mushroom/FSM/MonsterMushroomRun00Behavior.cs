using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomRun00Behavior : MonsterMushroomBehavior
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Mushroom.Agent.pathEndPosition == m_Mushroom.transform.position)
        {
            // 타겟을 공격해야하는 경우
            if (m_Mushroom.Target != null)
            {
                float currentDistanceWithTarget = Vector3.Distance(m_Mushroom.Target.transform.position, m_Mushroom.transform.position);

                if (ChooseAttackDecision(currentDistanceWithTarget) == MonsterMushroomDecision.None)
                {
                    Vector3 fromTarget = (m_Self.Target.transform.position - m_Self.transform.position);
                    Vector3 endPosition = m_Self.Target.transform.position;
                    endPosition = m_Self.Target.transform.position - (fromTarget.normalized * m_Attack01Data.AttackDistance);
                    m_Mushroom.SetWalkBehavior(endPosition);
                }
                    
            }
        }

    }
}
