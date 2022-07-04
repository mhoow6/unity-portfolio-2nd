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

            // ���ݹ��� �ȿ� �ִ� ���
            if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromNormal))
            {
                // �����Ÿ� �ȿ� �ִ� ���
                // ps.�Ÿ� = �ӷ� * �ð�
                if (Vector3.SqrMagnitude(fromMeToTarget) < Mathf.Pow(m_AttackData.BulletSpeed * m_AttackData.BulletLifeTime, 2))
                {
                    // ��� Ÿ���� �Ĵٺ��鼭 �����غ� �ܰ迡 �����Ѵ�.
                    m_Self.LookAtUntil(m_Self.Target.transform, () =>
                    {
                        Debug.Log($"{m_Self.name}�� �����ϴ� ��.. ����ð�({m_DecisionTimer}��)");

                        // ������ �ð� ����
                        if (m_DecisionTimer < decisionTime)
                            m_DecisionTimer += Time.deltaTime;
                        else
                        {
                            Debug.Log($"{m_Self.name}�� ������ ����. (�Ǵ�: ����)");

                            // ����
                            m_Self.ShootBullet();
                        }
                    });
                }
                else
                {
                    // ��� Ÿ���� �Ĵٺ��鼭 Ÿ������ �� �غ� �Ѵ�.
                    m_Self.LookAtUntil(m_Self.Target.transform, () =>
                    {
                        Debug.Log($"{m_Self.name}�� �����ϴ� ��.. ����ð�({m_DecisionTimer}��)");

                        // ������ �ð� ����
                        if (m_DecisionTimer < decisionTime)
                            m_DecisionTimer += Time.deltaTime;
                        else
                        {
                            Debug.Log($"{m_Self.name}�� ������ ����. (�Ǵ�: �پ��)");

                            // ����
                            m_Self.GoingTo(m_Self.Target.transform.position);
                        }
                    });
                }
            }
        }
    }
}
