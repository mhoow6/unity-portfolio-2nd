using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomIdle00Behavior : MonsterMushroomBehavior
{
    protected override AniType m_AniType => AniType.IDLE_0;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Mushroom.Decision == MonsterMushroomDecision.Sit)
            return;

        if (m_Self.Target != null)
        {
            // 타겟쪽으로 고개를 돌려준다
            Vector3 fromTarget = (m_Self.Target.transform.position - m_Self.transform.position);
            float decisionTime = m_BehaviorData.BehaviorDecisionTime;

            m_Mushroom.LookAtWith(m_Self.Target.transform, () =>
            {
                // 생각할 시간 측정
                if (m_DecisionTimer <= decisionTime)
                    m_DecisionTimer += Time.deltaTime;
                else if (m_DecisionTimer > decisionTime)
                {
                    // 결정하기
                    float currentDistanceWithTarget = Vector3.Distance(m_Self.Target.transform.position, m_Mushroom.transform.position);
                    if (ChooseAttackDecision(currentDistanceWithTarget) == MonsterMushroomDecision.None)
                    {
                        Vector3 endPosition = m_Self.Target.transform.position;
                        endPosition = m_Self.Target.transform.position - (fromTarget.normalized * m_BehaviorData.TargetKeepDistance);

                        m_Mushroom.SetWalkBehavior(endPosition);
                    }
                }
            });
        }
    }
}
