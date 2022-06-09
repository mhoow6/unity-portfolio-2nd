using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherRun00Behaviour : SparcherBehaviour
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_InputSystem.CharacterAttackInput)
            m_Player.AnimationJobs.Enqueue(AniType.ATTACK_0);
        else if (m_InputSystem.CharacterDashInput)
            m_Player.AnimationJobs.Enqueue(AniType.DASH_0);
        else if (m_InputSystem.CharacterUltiInput)
            m_Player.AnimationJobs.Enqueue(AniType.ATTACK_1);
        else if (m_InputSystem.CharacterMoveInput.magnitude != 0 && m_Player.Moveable)
            m_Player.AnimationJobs.Enqueue(AniType.RUN_0);
        else
            m_Player.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
