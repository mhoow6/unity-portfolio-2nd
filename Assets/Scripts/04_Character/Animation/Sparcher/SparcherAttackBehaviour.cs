using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherAttackBehaviour : AnimationBehaviour
{
    protected override AniType m_AniType => AniType.ATTACK_0;
    Player m_Player;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player = GameManager.Instance.Player;
        m_Player.Controlable = false;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 시간되면 알아서 IDLE로 돌아간다.
        if (m_CurrentAnimationTime > 0.75f)
            animator.SetInteger("AniType", (int)AniType.IDLE_0);
    }

    protected override void OnAnimationExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player.Controlable = true;
    }
}
