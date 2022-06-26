using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherBehaviour : AnimationBehaviour
{
    protected InputSystem m_InputSystem;
    protected Player m_Player;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player = StageManager.Instance.Player;
        m_InputSystem = GameManager.InputSystem;
    }
}
