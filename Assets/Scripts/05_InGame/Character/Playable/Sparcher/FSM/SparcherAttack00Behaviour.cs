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
        // 오토 타겟팅이 켜져있으면 공격거리내의 몬스터에게 몸이 회전한다.
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

        // 68.6%에 화살 쏘기
        if (m_CurrentAnimationTime > 0.686f && !m_Shoot)
        {
            m_Shoot = true;
            m_Sparcher.ShootArrow();
        }

        // 자유롭게 행동할 수 있는 시간은 애니메이션이 80% 완료된 이후
        if (m_CurrentAnimationTime > 0.80f && !m_GoIdle)
        {
            m_GoIdle = true;

            m_Player.Moveable = true;
            m_Player.AnimationJobs.Enqueue(AniType.IDLE_0);
        }
            
    }
}
