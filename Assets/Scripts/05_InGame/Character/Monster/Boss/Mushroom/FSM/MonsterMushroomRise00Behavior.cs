using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomRise00Behavior : MonsterMushroomBehavior
{
    protected override void OnAnimationExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Mushroom.SetIdleBehavior();
    }
}
