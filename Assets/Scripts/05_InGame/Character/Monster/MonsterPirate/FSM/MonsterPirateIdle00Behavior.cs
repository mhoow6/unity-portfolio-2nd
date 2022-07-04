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

        if (m_Self.Target != null)
        {
            Vector3 forward = m_Self.transform.forward;
            Vector3 fromTarget = (m_Self.Target.transform.position - m_Self.transform.position);
            Vector3 fromTargetNormalized = fromTarget.normalized;

            float halfAngle = m_BehaviorData.EnemyDetectAngle * 0.5f * Mathf.Deg2Rad;
            float decisionTime = m_BehaviorData.BehaviorDecisionTime;
            float attackRange = m_AttackData.BulletSpeed * m_AttackData.BulletLifeTime;

            // ���ݹ��� �ȿ� �ִ� ���
            if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromTargetNormalized))
            {
                if (m_FirstLookAt)
                {
                    // �����Ÿ� �ȿ� �ִ� ���
                    // ps.�Ÿ� = �ӷ� * �ð�
                    if (Vector3.SqrMagnitude(fromTarget) <= Mathf.Pow(attackRange, 2))
                    {
                        // ��� Ÿ���� �Ĵٺ��鼭 �����غ� �ܰ迡 �����Ѵ�.
                        m_Self.LookAtWith(m_Self.Target.transform, () =>
                        {
                            // ������ �ð� ����
                            if (m_DecisionTimer < decisionTime)
                                m_DecisionTimer += Time.deltaTime;
                            else
                            {
                                // ����
                                m_Self.ShootBullet();
                            }
                        });
                    }
                    else
                    {
                        // ��� Ÿ���� �Ĵٺ��鼭 Ÿ������ �� �غ� �Ѵ�.
                        m_Self.LookAtWith(m_Self.Target.transform, () =>
                        {
                            // ������ �ð� ����
                            if (m_DecisionTimer < decisionTime)
                                m_DecisionTimer += Time.deltaTime;
                            else
                            {
                                Vector3 stopPosition = m_Self.Target.transform.position - (fromTargetNormalized * attackRange);
                                m_Self.GoingTo(stopPosition);
                            }
                        });
                    }
                }
            }

            if (!m_LookAtLerp)
            {
                m_LookAtLerp = true;
                // Idle ���¿��� �� ���� �������� ������ ��������� �Ĵٺ��� �����ؾ���.
                m_Self.LookAtLerp(Quaternion.LookRotation(fromTarget), 1f,
                    () =>
                    {
                        m_FirstLookAt = true;
                    });
            }
        }
    }
}
