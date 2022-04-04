using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Character_StateMachineBehaviour : StateMachineBehaviour
{
    protected virtual AniType m_type { get; }

    float m_currentAnimationTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var character = GameManager.Instance.Player.CurrentCharacter;
        character.SpeakDialogueAtMainMenu(m_type);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 시간되면 알아서 IDLE로 돌아간다.
        m_currentAnimationTime = stateInfo.normalizedTime % 1;
        if (m_currentAnimationTime > 0.75f)
        {
            animator.SetInteger("AniType", 0);
        }
    }
}
