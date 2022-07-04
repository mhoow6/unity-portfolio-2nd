using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateRun00Behavior : MonsterPirateBehavior
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Self.Agent.pathEndPosition == m_Self.transform.position)
        {
            if (m_CurrentAnimationTime > 0.80f)
                m_Self.ShootBullet();
        }
    }
}
