using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateIdle00Behavior : MonsterPirateBehavior
{
    float m_DecisionTimer;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_DecisionTimer = 0f;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Self.Target != null)
        {
            Vector3 forward = m_Self.transform.forward;
            Vector3 fromMeToTarget = (m_Self.Target.transform.position - m_Self.transform.position);
            Vector3 fromNormal = fromMeToTarget.normalized;

            float halfAngle = m_BehaviorData.EnemyDetectAngle * 0.5f * Mathf.Deg2Rad;
            float decisionTime = m_BehaviorData.BehaviorDecisionTime;

            // 공격범위 안에 있는 경우
            if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromNormal))
            {
                // 사정거리 안에 있는 경우
                // ps.거리 = 속력 * 시간
                if (Vector3.SqrMagnitude(fromMeToTarget) < Mathf.Pow(m_AttackData.BulletSpeed * m_AttackData.BulletLifeTime, 2))
                {
                    // 계속 타겟을 쳐다보면서 공격준비 단계에 돌입한다.
                    m_Self.LookAtUntil(m_Self.Target.transform, () =>
                    {
                        Debug.Log($"{m_Self.name}이 생각하는 중.. 경과시간({m_DecisionTimer}초)");

                        // 생각할 시간 측정
                        if (m_DecisionTimer < decisionTime)
                            m_DecisionTimer += Time.deltaTime;
                        else
                        {
                            Debug.Log($"{m_Self.name}이 생각을 끝냄. (판단: 공격)");

                            // 공격
                            m_Self.ShootBullet();
                        }
                    });
                }
                else
                {
                    // 계속 타겟을 쳐다보면서 타겟한테 갈 준비를 한다.
                    m_Self.LookAtUntil(m_Self.Target.transform, () =>
                    {
                        Debug.Log($"{m_Self.name}이 생각하는 중.. 경과시간({m_DecisionTimer}초)");

                        // 생각할 시간 측정
                        if (m_DecisionTimer < decisionTime)
                            m_DecisionTimer += Time.deltaTime;
                        else
                        {
                            Debug.Log($"{m_Self.name}이 생각을 끝냄. (판단: 뛰어가기)");

                            // 공격
                            m_Self.GoingTo(m_Self.Target.transform.position);
                        }
                    });
                }
            }
        }
    }
}
