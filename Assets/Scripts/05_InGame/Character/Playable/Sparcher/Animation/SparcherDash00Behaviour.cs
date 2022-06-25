using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class SparcherDash00Behaviour : SparcherBehaviour
{
    protected override AniType m_AniType => AniType.DASH_0;

    SparcherDashData m_Data;
    Vector3 m_Destination;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        // JSON 가져오기
        if (m_Data == null)
        {
            if (JsonManager.Instance.JsonDatas.TryGetValue(2002, out JsonDatable json))
            {
                SparcherDashData dashData = json as SparcherDashData;
                m_Data = dashData;
            }
        }
        
        if (m_Data != null)
            m_Destination = m_Player.CurrentCharacter.transform.position + (m_Player.CurrentCharacter.transform.forward * m_Data.DashDistance);

        m_Player.CurrentCharacter.Invulnerable = true;
        m_Player.Moveable = false;

        m_Player.SmoothlyMovingTo(m_Destination, m_Data.ArriveTime);
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (!m_Player.SmoothlyMoving)
        {
            m_Player.CurrentCharacter.Invulnerable = false;
            m_Player.Moveable = true;

            m_Player.AnimationJobs.Enqueue(AniType.IDLE_0);
        }
            
    }
}
