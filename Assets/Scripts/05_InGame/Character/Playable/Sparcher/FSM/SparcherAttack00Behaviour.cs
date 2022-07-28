using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SparcherAttack00Behaviour : SparcherBehaviour
{
    protected override AniType m_AniType => AniType.ATTACK_0;
    bool m_Shoot;
    bool m_GoIdle;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_Player.Moveable = false;
        m_Shoot = false;
        m_GoIdle = false;

        var skillData = JsonManager.Instance.JsonDatas[Character.GetAInputDataIndex(m_Sparcher.Code)] as SparcherBasicAttackData;
        // ���� Ÿ������ ���������� ���ݰŸ����� ���Ϳ��� ���� ȸ���Ѵ�.
        if (GameManager.GameSettings.AutoTargeting)
        {
            var ordered = StageManager.Instance.Monsters.OrderBy(m => Vector3.Distance(m.transform.position, m_Sparcher.transform.position)).ToList();
            var find = ordered.Find(m => Vector3.Distance(m.transform.position, m_Sparcher.transform.position) <= skillData.AutoTargetDetectRange);
            if (find != null)
            {
                m_Sparcher.Target = find;
                m_Sparcher.transform.LookAt(find.transform);
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
            m_Sparcher.ShootArrow();
        }

        // �����Ӱ� �ൿ�� �� �ִ� �ð��� �ִϸ��̼��� 80% �Ϸ�� ����
        if (m_CurrentAnimationTime > 0.80f && !m_GoIdle)
        {
            m_GoIdle = true;

            m_Player.Moveable = true;
            m_Player.AnimationJobs.Enqueue(AniType.IDLE_0);
        }
            
    }
}
