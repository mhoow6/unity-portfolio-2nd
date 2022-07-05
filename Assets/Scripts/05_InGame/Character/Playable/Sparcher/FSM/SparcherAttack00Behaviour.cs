using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SparcherAttack00Behaviour : SparcherBehaviour
{
    protected override AniType m_AniType => AniType.ATTACK_0;
    bool m_Shoot;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_Player = StageManager.Instance.Player;
        m_Player.Controlable = false;
        var skillData = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Self.Code)] as SparcherBasicAttackData;

        // ���� Ÿ������ ���������� ���ݰŸ����� ���Ϳ��� ���� ȸ���Ѵ�.
        if (GameManager.GameSettings.AutoTargeting)
        {
            var ordered = StageManager.Instance.Monsters.OrderBy(m => Vector3.Distance(m.transform.position, m_Self.transform.position)).ToList();
            var find = ordered.Find(m => Vector3.Distance(m.transform.position, m_Self.transform.position) <= skillData.AutoTargetDetectRange);
            if (find != null)
            {
                m_Self.Target = find;
                m_Self.transform.LookAt(find.transform);
            }
        }
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        // 68.6%�� ȭ�� ���
        if (m_CurrentAnimationTime > 0.686f && !m_Shoot)
        {
            m_Shoot = true;
            m_Self.Attack();
        }

        // �����Ӱ� �ൿ�� �� �ִ� �ð��� �ִϸ��̼��� 80% �Ϸ�� ����
        if (m_CurrentAnimationTime > 0.80f)
            m_Player.AnimationJobs.Enqueue(AniType.IDLE_0);
    }

    protected override void OnAnimationExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player.Controlable = true;
        m_Shoot = false;
    }
}
