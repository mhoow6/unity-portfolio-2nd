using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomIdle00Behavior : MonsterMushroomBehavior
{
    protected override AniType m_AniType => AniType.IDLE_0;
    bool m_LookAt;
    IsTargetInParam m_IsTargetInParam;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Self.Target != null)
        {
            IsTargetInParam isTargetInParam = new IsTargetInParam()
            {
                Target = m_Self.Target,
                DetectAngle = m_BehaviorData.EnemyDetectAngle,
                DetectRange = m_BehaviorData.EnemyDetectRange
            };
            if (IsTargetIn(isTargetInParam))
            {
                // 타겟쪽으로 고개를 돌려준다
                Vector3 fromTarget = (m_Self.Target.transform.position - m_Self.transform.position);
                float decisionTime = m_BehaviorData.BehaviorDecisionTime;
                m_Mushroom.LookAtWith(m_Self.Target.transform, () =>
                {
                    Debug.Log($"생각할 수 있는 시간: {m_DecisionTimer}");

                    // 생각할 시간 측정
                    if (m_DecisionTimer <= decisionTime)
                        m_DecisionTimer += Time.deltaTime;
                    else if (m_DecisionTimer > decisionTime)
                    {
                        // 결정하기
                        float currentDistanceWithTarget = Vector3.Distance(isTargetInParam.Target.transform.position, m_Mushroom.transform.position);
                        if (ChooseAttackDecision(currentDistanceWithTarget) == MonsterMushroomDecision.None)
                        {
                            Vector3 endPosition = m_Self.Target.transform.position;
                            endPosition = m_Self.Target.transform.position - (fromTarget.normalized * m_Attack01Data.AttackDistance);

                            m_Mushroom.SetWalkBehavior(endPosition);
                        }
                    }
                });
            }
            else
            {
                if (m_LookAt == false)
                {
                    Vector3 fromTarget = (m_Self.Target.transform.position - m_Self.transform.position);
                    m_LookAt = true;
                    m_Mushroom.LookAtLerp(Quaternion.LookRotation(fromTarget), m_BehaviorData.EnemyLookAtSpeed, lookAtDoneCallback: () =>
                    {
                        m_LookAt = false;
                    });
                }
            }
        }
        
        
    }
}
