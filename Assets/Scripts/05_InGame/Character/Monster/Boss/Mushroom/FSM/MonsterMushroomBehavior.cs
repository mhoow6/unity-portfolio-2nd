using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class MonsterMushroomBehavior : CharacterBehavior
{
    protected MonsterMushroom m_Mushroom;
    protected MonsterMushroomBehaviorData m_BehaviorData;
    protected MonsterMushroomAttackData m_AttackData;
    protected float m_DecisionTimer;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        if (m_Mushroom == null)
        {
            m_Mushroom = animator.GetComponent<MonsterMushroom>();
            m_BehaviorData = m_Mushroom.BehaviorData as MonsterMushroomBehaviorData;
            m_AttackData = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Mushroom.Code)] as MonsterMushroomAttackData;
        }

        m_DecisionTimer = 0f;
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        if (m_Mushroom.Hp <= 0)
            m_Mushroom.AnimationJobs.Enqueue(AniType.DEAD_0);
    }
}
