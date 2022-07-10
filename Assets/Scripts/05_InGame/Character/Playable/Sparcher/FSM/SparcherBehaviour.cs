using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherBehaviour : CharacterBehavior
{
    protected Player m_Player;
    protected InputSystem m_InputSystem;
    protected Sparcher m_Sparcher;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_Player = StageManager.Instance.Player;
        m_Sparcher = m_Player.CurrentCharacter as Sparcher;
        m_InputSystem = GameManager.InputSystem;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Sparcher.Hp <= 0)
            m_Player.AnimationJobs.Enqueue(AniType.DEAD_0);
    }
}
