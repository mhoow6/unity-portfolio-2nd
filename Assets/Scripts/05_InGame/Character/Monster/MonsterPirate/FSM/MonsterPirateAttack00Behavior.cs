using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateAttack00Behavior : MonsterPirateBehavior
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        // 자유롭게 행동할 수 있는 시간은 애니메이션이 80% 완료된 이후
        if (m_CurrentAnimationTime > 0.80f)
            m_Self.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
