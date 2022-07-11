using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomAttack03Behavior : MonsterMushroomBehavior
{
    bool m_attack03;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_attack03 = false;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Mushroom.Decision == MonsterMushroomDecision.Sit)
            return;

        if (m_CurrentAnimationTime > 0.71f && m_attack03 == false)
        {
            m_attack03 = true;
            m_Mushroom.Attack03();
        }

        // 돌아가기
        if (m_CurrentAnimationTime > 0.80f)
            m_Mushroom.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
