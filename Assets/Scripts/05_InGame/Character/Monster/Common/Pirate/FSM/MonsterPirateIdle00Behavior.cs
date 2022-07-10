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
            // �Ÿ� = �ӷ� * �ð�
            float attackRange = m_AttackData.BulletSpeed * m_AttackData.BulletLifeTime;

            // ���ݹ��� �ȿ� �ִ� ���
            if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromTargetNormalized))
            {
                if (m_FirstLookAt)
                {
                    // �����Ÿ� �ȿ� �ִ� ��� 
                    if (Mathf.Round(Vector3.SqrMagnitude(fromTarget)) <= Mathf.Pow(attackRange, 2))
                    {
                        // ��� Ÿ���� �Ĵٺ��鼭 �����غ� �ܰ迡 �����Ѵ�.
                        m_Pirate.LookAtWith(m_Pirate.Target.transform, () =>
                        {
                            Debug.Log($"������ �����ϴ� �ð�: {m_DecisionTimer}");

                            // ������ �ð� ����
                            if (m_DecisionTimer <= decisionTime)
                                m_DecisionTimer += Time.deltaTime;
                            else if (m_DecisionTimer > decisionTime)
                            {
                                // ����
                                m_Pirate.SetAttackBehavior();
                            }
                        });
                    }
                    else
                    {
                        // ��� Ÿ���� �Ĵٺ��鼭 Ÿ������ �� �غ� �Ѵ�.
                        m_Pirate.LookAtWith(m_Pirate.Target.transform, () =>
                        {
                            // ������ �ð� ����
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

            // Idle ���¿��� �� ���� �������� ������ ��������� �Ĵٺ��� �����ؾ���.
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
