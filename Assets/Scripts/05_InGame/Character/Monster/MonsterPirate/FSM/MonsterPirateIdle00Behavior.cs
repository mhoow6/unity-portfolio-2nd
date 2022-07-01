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

            // ���ݹ��� �ȿ� �ִ� ���
            if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, targetNormal))
            {
                // �����Ÿ� �ȿ� �ִ� ���
                // ps. �Ÿ� = �ӷ� * �ð�
                if (m_LookAt == false)
                {
                    if (Vector3.SqrMagnitude(target) < Mathf.Pow(m_AttackData.BulletSpeed * m_AttackData.BulletLifeTime, 2))
                    {
                        // õõ�� ���� ������ Ÿ���� �ٶ� ������ ���.
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
