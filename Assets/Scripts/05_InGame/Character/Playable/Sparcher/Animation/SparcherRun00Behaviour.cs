using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherRun00Behaviour : SparcherBehaviour
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_InputSystem.PressAButton)
            m_Player.AnimationJobs.Enqueue(AniType.ATTACK_0);
        else if (m_InputSystem.PressXButton)
            m_Player.AnimationJobs.Enqueue(AniType.DASH_0);
        else if (m_InputSystem.CharacterMoveInput.magnitude != 0 && m_Player.Moveable)
            m_Player.AnimationJobs.Enqueue(AniType.RUN_0);
        else
            m_Player.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
