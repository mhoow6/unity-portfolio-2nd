using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class MonsterMushroomBehavior : CharacterBehavior
{
    protected MonsterMushroom m_Mushroom;
    protected MonsterMushroomBehaviorData m_BehaviorData;
    protected MonsterMushroomAttack01Data m_Attack01Data;
    protected MonsterMushroomAttack02Data m_Attack02Data;
    protected MonsterMushroomAttack03Data m_Attack03Data;

    protected float m_DecisionTimer;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        if (m_Mushroom == null)
        {
            m_Mushroom = animator.GetComponent<MonsterMushroom>();
            m_BehaviorData = m_Mushroom.BehaviorData as MonsterMushroomBehaviorData;
            m_Attack01Data = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Mushroom.Code)] as MonsterMushroomAttack01Data;
            m_Attack02Data = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Mushroom.Code) + 1] as MonsterMushroomAttack02Data;
            m_Attack03Data = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(m_Mushroom.Code) + 2] as MonsterMushroomAttack03Data;
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
