using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherBehaviour : AnimationBehaviour
{
    protected Player m_Player;
    protected InputSystem m_InputSystem;
    protected Sparcher m_Sparcher;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player = StageManager.Instance.Player;
        m_Sparcher = m_Player.CurrentCharacter as Sparcher;
        m_InputSystem = GameManager.InputSystem;
    }
}