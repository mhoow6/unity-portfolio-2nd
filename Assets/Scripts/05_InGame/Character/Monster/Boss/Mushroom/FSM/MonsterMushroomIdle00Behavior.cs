using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMushroomIdle00Behavior : MonsterMushroomBehavior
{
    protected override AniType m_AniType => AniType.IDLE_0;
    bool m_LookAt;
    IsTargetInParam m_IsTargetInParam;

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        IsTargetInParam isTargetInParam = new IsTargetInParam()
        {
            Target = m_Self.Target,
            DetectAngle = m_BehaviorData.EnemyDetectAngle,
            DetectRange = m_BehaviorData.EnemyDetectRange
        };
        if (IsTargetIn(isTargetInParam))
        {
            
        }
        else
        {
            // 타겟쪽으로 고개를 돌려준다
            if (m_LookAt == false)
            {
                Vector3 fromTarget = (m_Self.Target.transform.position - m_Self.transform.position);
                m_LookAt = true;
                m_Mushroom.LookAtLerp(Quaternion.LookRotation(fromTarget), m_BehaviorData.EnemyLookAtSpeed, lookAtDoneCallback: () =>
                {
                    m_LookAt = false;

                    // 고개를 다 돌리고 나면 타겟과의 거리를 재서
                    float currentDistanceWithTarget = Vector3.Distance(isTargetInParam.Target.transform.position, m_Mushroom.transform.position);

                    // 점프 공격으로 공격이 맞을 것 같으면 점프공격을 하고, 그렇지 않으면 1,2번중 택
                    if (currentDistanceWithTarget <= m_Attack03Data.ThrustDistance)
                        m_Mushroom.SetAttack03Behavior();
                    else
                    {
                        int random = UnityEngine.Random.Range(0, 2);
                        if (random > 0)
                            m_Mushroom.SetAttack02Behavior();
                        else
                            m_Mushroom.SetAttackBehavior();
                    }
                });
            }
        }
        
    }
}
