using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomAttack01Behavior : MonsterMushroomBehavior
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        // 돌아가기
        if (m_CurrentAnimationTime > 0.80f)
            m_Mushroom.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
