using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherIdle00Behaviour : SparcherBehaviour
{
    protected override AniType m_AniType => AniType.IDLE_0;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_InputSystem.PressAButton)
            m_Player.AnimationJobs.Enqueue(AniType.ATTACK_0);
        else if (m_InputSystem.PressXButton)
            m_Player.AnimationJobs.Enqueue(AniType.DASH_0);
        else if (m_InputSystem.LeftStickInput.magnitude != 0 && m_Player.Moveable)
            m_Player.AnimationJobs.Enqueue(AniType.RUN_0);
        else if (m_InputSystem.PressYButton)
            m_Player.AnimationJobs.Enqueue(AniType.JUMP_0);
    }
}
