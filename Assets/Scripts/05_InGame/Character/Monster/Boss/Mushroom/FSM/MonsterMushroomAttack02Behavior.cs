using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomAttack02Behavior : MonsterMushroomBehavior
{
    bool m_attack02;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_attack02 = false;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_CurrentAnimationTime > 0.5f && m_attack02 == false)
        {
            m_attack02 = true;
            m_Mushroom.Attack02();
        }

        // 돌아가기
        if (m_CurrentAnimationTime > 0.80f)
            m_Mushroom.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
