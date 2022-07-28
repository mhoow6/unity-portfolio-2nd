using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBehavior : CharacterBehavior
{
    protected Knight m_Knight;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_Knight = StageManager.Instance.Player.CurrentCharacter as Knight;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Knight.Hp <= 0)
            StageManager.Instance.Player.AnimationJobs.Enqueue(AniType.DEAD_0);
    }
}
