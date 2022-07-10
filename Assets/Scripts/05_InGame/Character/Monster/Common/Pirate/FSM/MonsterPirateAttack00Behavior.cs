using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateAttack00Behavior : MonsterPirateBehavior
{
    bool m_Shoot;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);
        m_Shoot = false;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        // 35%�뿡 �������
        if (m_CurrentAnimationTime > 0.35f && !m_Shoot)
        {
            m_Shoot = true;
            m_Pirate.ShootBullet();
        }

        // ���ư���
        if (m_CurrentAnimationTime > 0.80f)
            m_Pirate.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
