using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAttack00Behavior : KnightBehavior
{
    protected override AniType m_AniType => AniType.ATTACK_0;

    bool m_Attack;
    bool m_GoIdle;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_GoIdle = false;
        m_Attack = false;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);
        var player = StageManager.Instance.Player;

        // 68.6%에 공격하기
        if (m_CurrentAnimationTime > 0.686f && !m_Attack)
        {
            m_Attack = true;
            m_Knight.SlashSword();
        }

        // 자유롭게 행동할 수 있는 시간은 애니메이션이 80% 완료된 이후
        if (m_CurrentAnimationTime > 0.80f && !m_GoIdle)
        {
            m_GoIdle = true;

            player.Moveable = true;
            player.AnimationJobs.Enqueue(AniType.IDLE_0);
        }

    }
}
