using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : AnimationBehaviour
{
    protected Character m_Self;
    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_Self == null)
            m_Self = animator.GetComponent<Character>();
    }

    protected bool IsTargetIn(IsTargetInParam param)
    {
        if (m_Self.Target == null)
            return false;

        Vector3 forward = m_Self.transform.forward;
        Vector3 fromTarget = (m_Self.Target.transform.position - m_Self.transform.position);
        Vector3 fromTargetNormalized = fromTarget.normalized;

        float halfAngle = param.AttackAngle * 0.5f * Mathf.Deg2Rad;
        float attackRange = param.AttackRange;

        // 공격범위 안에 있는 경우
        if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromTargetNormalized))
        {
            // 사정거리 안에 있는 경우 
            if (Mathf.Round(Vector3.SqrMagnitude(fromTarget)) <= Mathf.Pow(attackRange, 2))
            {
                OnTargetIn(param);
                return true;
            }
            return false;
        }
        else
            return false;
    }

    protected virtual void OnTargetIn(IsTargetInParam param) { }
}
