using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateIdle00Behavior : MonsterPirateBehavior
{
    bool m_FirstLookAt;
    bool m_LookAtLerp;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_FirstLookAt = false;
        m_LookAtLerp = false;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Pirate.Target != null)
        {
            Vector3 forward = m_Pirate.transform.forward;
            Vector3 fromTarget = (m_Pirate.Target.transform.position - m_Pirate.transform.position);
            Vector3 fromTargetNormalized = fromTarget.normalized;

            float halfAngle = m_BehaviorData.EnemyDetectAngle * 0.5f * Mathf.Deg2Rad;
            float decisionTime = m_BehaviorData.BehaviorDecisionTime;
            // 거리 = 속력 * 시간
            float attackRange = m_AttackData.BulletSpeed * m_AttackData.BulletLifeTime;

            // 공격범위 안에 있는 경우
            if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromTargetNormalized))
            {
                if (m_FirstLookAt)
                {
                    // 사정거리 안에 있는 경우 
                    if (Mathf.Round(Vector3.SqrMagnitude(fromTarget)) <= Mathf.Pow(attackRange, 2))
                    {
                        // 계속 타겟을 쳐다보면서 공격준비 단계에 돌입한다.
                        m_Pirate.LookAtWith(m_Pirate.Target.transform, () =>
                        {
                            Debug.Log($"해적이 생각하는 시간: {m_DecisionTimer}");

                            // 생각할 시간 측정
                            if (m_DecisionTimer <= decisionTime)
                                m_DecisionTimer += Time.deltaTime;
                            else if (m_DecisionTimer > decisionTime)
                            {
                                // 공격
                                m_Pirate.SetAttackBehavior();
                            }
                        });
                    }
                    else
                    {
                        // 계속 타겟을 쳐다보면서 타겟한테 갈 준비를 한다.
                        m_Pirate.LookAtWith(m_Pirate.Target.transform, () =>
                        {
                            // 생각할 시간 측정
                            if (m_DecisionTimer <= decisionTime)
                                m_DecisionTimer += Time.deltaTime;
                            else if (m_DecisionTimer > decisionTime)
                            {
                                Vector3 stopPosition = m_Pirate.Target.transform.position - (fromTargetNormalized * attackRange);
                                m_Pirate.SetWalkBehavior(stopPosition);
                            }
                        });
                    }
                }
            }

            // Idle 상태에서 한 번도 봐준적이 없으면 어느정도는 쳐다보고 시작해야함.
            if (!m_LookAtLerp)
            {
                m_LookAtLerp = true;
                
                m_Pirate.LookAtLerp(Quaternion.LookRotation(fromTarget), 1f,
                    () =>
                    {
                        m_FirstLookAt = true;
                    });
            }
        }
    }
}
