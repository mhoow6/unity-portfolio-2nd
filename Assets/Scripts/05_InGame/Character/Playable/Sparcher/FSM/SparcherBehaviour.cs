using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherBehaviour : AnimationBehaviour
{
    protected Player m_Player;
    protected InputSystem m_InputSystem;
    protected Sparcher m_Self;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player = StageManager.Instance.Player;
        m_Self = m_Player.CurrentCharacter as Sparcher;
        m_InputSystem = GameManager.InputSystem;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_Self.Hp <= 0)
            m_Player.AnimationJobs.Enqueue(AniType.DEAD_0);
    }
}
