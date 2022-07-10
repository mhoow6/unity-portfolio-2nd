using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomAttack01Behavior : MonsterMushroomBehavior
{
    bool m_attack01;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_attack01 = false;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_CurrentAnimationTime > 0.32f && m_attack01 == false)
        {
            m_attack01 = true;
            m_Mushroom.Attack01();
        }

        // 돌아가기
        if (m_CurrentAnimationTime > 0.80f)
            m_Mushroom.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
