using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnAnimationEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_CurrentAnimationTime = stateInfo.normalizedTime % 1;
        if (!animator.IsInTransition(layerIndex))
            OnAnimationUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnAnimationExit(animator, stateInfo, layerIndex);
    }

    // -----------------------------------------------------------------------

    protected float m_CurrentAnimationTime;
    protected virtual AniType m_AniType { get; }
    protected virtual void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    protected virtual void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    protected virtual void OnAnimationExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
}
