using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : AnimationBehaviour
{
    protected Character m_Self;
    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_Self == null)
            m_Self = animator.GetComponent<Character>();
    }
}
