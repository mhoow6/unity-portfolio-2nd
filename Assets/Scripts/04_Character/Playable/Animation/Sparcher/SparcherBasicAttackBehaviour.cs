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

        // ���� Ÿ������ ���������� ���ݰŸ����� ���Ϳ��� ���� ȸ���Ѵ�.
        if (GameManager.Instance.AutoTargeting)
        {
            var find = StageManager.Instance.Monsters.Find(m => Vector3.Distance(m.transform.position, sparcher.transform.position) <= skillData.AutoTargetDetectRange);
            if (find != null)
                sparcher.transform.LookAt(find.transform);
        }
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �ð��Ǹ� �˾Ƽ� IDLE�� ���ư���.
        if (m_CurrentAnimationTime > 0.5f)
            animator.SetInteger("AniType", (int)AniType.IDLE_0);
    }

    protected override void OnAnimationExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player.Controlable = true;
    }
}
