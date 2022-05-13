using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBehaviour : AnimationBehaviour
{
    float timer = 0f;
    const float INVISIBLE_TIME = 3f;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > INVISIBLE_TIME)
        {
            Destroy(animator.gameObject);
            timer = 0f;
        }
    }
}
