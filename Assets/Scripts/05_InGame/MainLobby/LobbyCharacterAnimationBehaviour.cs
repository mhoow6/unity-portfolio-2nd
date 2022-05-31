using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterAnimationBehaviour : AnimationBehaviour
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 시간되면 알아서 IDLE로 돌아간다.
        if (m_CurrentAnimationTime > 0.75f)
        {
            animator.SetInteger("AniType", (int)AniType.IDLE_0);
        }
    }
}
