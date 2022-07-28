using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightRun00Behavior : KnightBehavior
{
    protected override AniType m_AniType => AniType.RUN_0;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);
        var inputsystem = GameManager.InputSystem;
        var player = StageManager.Instance.Player;

        if (inputsystem.PressAButton)
            player.AnimationJobs.Enqueue(AniType.ATTACK_0);
        else if (inputsystem.LeftStickInput.magnitude != 0 && player.Moveable)
            player.AnimationJobs.Enqueue(AniType.RUN_0);
        else if (inputsystem.PressYButton)
            player.AnimationJobs.Enqueue(AniType.JUMP_0);
        else if (inputsystem.LeftStickInput.magnitude == 0)
            player.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
