using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateIdle00Behavior : MonsterPirateBehavior
{
    bool m_LookAt;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_Self.Target != null)
        {
            Vector3 forward = m_Self.transform.forward;
            Vector3 target = (m_Self.Target.transform.position - m_Self.transform.position);
            Vector3 targetNormal = target.normalized;

            float halfAngle = m_BehaviorData.EnemyDetectAngle * 0.5f * Mathf.Deg2Rad;

            // 공격범위 안에 있는 경우
            if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, targetNormal))
            {
                // 사정거리 안에 있는 경우
                // ps. 거리 = 속력 * 시간
                if (m_LookAt == false)
                {
                    if (Vector3.SqrMagnitude(target) < Mathf.Pow(m_AttackData.BulletSpeed * m_AttackData.BulletLifeTime, 2))
                    {
                        // 천천히 고개를 돌려서 타겟을 바라본 다음에 쏜다.
                        m_Self.LookAtLerp(Quaternion.FromToRotation(m_Self.transform.forward, target), () =>
                        {
                            m_Self.AnimationJobs.Enqueue(AniType.ATTACK_0);
                            m_LookAt = false;
                        });
                        m_LookAt = true;
                    }
                }
                
            }
            else
            {
                
            }
        }
    }
}
