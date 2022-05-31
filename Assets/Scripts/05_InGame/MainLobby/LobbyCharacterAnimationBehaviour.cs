using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterAnimationBehaviour : AnimationBehaviour
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �ð��Ǹ� �˾Ƽ� IDLE�� ���ư���.
        if (m_CurrentAnimationTime > 0.75f)
        {
            animator.SetInteger("AniType", (int)AniType.IDLE_0);
        }
    }
}
