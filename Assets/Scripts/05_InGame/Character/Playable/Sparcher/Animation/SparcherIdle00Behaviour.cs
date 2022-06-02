using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherIdle00Behaviour : SparcherBehaviour
{
    protected override AniType m_AniType => AniType.IDLE_0;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_System.CharacterAttackInput)
            m_Player.AnimationJobs.Enqueue(AniType.ATTACK_0);
        else if (m_System.CharacterDashInput)
            m_Player.AnimationJobs.Enqueue(AniType.DASH_0);
        else if (m_System.CharacterUltiInput)
            m_Player.AnimationJobs.Enqueue(AniType.ATTACK_1);
        else if (m_System.CharacterMoveInput.magnitude != 0 && m_Player.Moveable)
            m_Player.AnimationJobs.Enqueue(AniType.RUN_0);
    }
}