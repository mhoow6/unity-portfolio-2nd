using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherBasicAttackBehaviour : AnimationBehaviour
{
    protected override AniType m_AniType => AniType.ATTACK_0;
    Player m_Player;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player = GameManager.Instance.Player;
        m_Player.Controlable = false;

        var sparcher = m_Player.CurrentCharacter as Sparcher;
        var skillData = JsonManager.Instance.JsonDatas[sparcher.ATTACK_INDEX] as SparcherBasicAttackData;

        // 오토 타겟팅이 켜져있으면 공격거리내의 몬스터에게 몸이 회전한다.
        if (GameManager.Instance.AutoTargeting)
        {
            var find = StageManager.Instance.Monsters.Find(m => Vector3.Distance(m.transform.position, sparcher.transform.position) <= skillData.AutoTargetDetectRange);
            if (find != null)
                sparcher.transform.LookAt(find.transform);
        }
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 시간되면 알아서 IDLE로 돌아간다.
        if (m_CurrentAnimationTime > 0.5f)
            animator.SetInteger("AniType", (int)AniType.IDLE_0);
    }

    protected override void OnAnimationExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player.Controlable = true;
    }
}
