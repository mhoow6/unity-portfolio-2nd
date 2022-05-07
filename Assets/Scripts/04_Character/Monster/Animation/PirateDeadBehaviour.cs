using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateDeadBehaviour : AnimationBehaviour
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_CurrentAnimationTime > 0.99f)
            Destroy(animator.gameObject);
    }
}
