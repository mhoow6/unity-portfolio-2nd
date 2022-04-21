using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSparcherStateMachineBehaviour : StateMachineBehaviour
{
    protected virtual AniType m_AniType { get; }

    float m_CurrentAnimationTime;
    //MainMenuUI m_MainMenuUI;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var character = GameManager.Instance.Player.CurrentCharacter;
        //character.SpeakDialogueAtMainMenu(m_Type);

        //var ui = GameManager.Instance.UISystem.CurrentWindow as MainMenuUI;
        //m_MainMenuUI = ui;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 시간되면 알아서 IDLE로 돌아간다.
        m_CurrentAnimationTime = stateInfo.normalizedTime % 1;
        if (m_CurrentAnimationTime > 0.75f)
        {
            animator.SetInteger("AniCode", (int)AniType.IDLE_0);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (m_MainMenuUI != null)
        //    m_MainMenuUI.CharacterDialog.gameObject.SetActive(false);
    }
}
