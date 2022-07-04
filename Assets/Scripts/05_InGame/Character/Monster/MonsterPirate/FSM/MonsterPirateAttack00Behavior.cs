using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPirateAttack00Behavior : MonsterPirateBehavior
{
    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationUpdate(animator, stateInfo, layerIndex);

        // �����Ӱ� �ൿ�� �� �ִ� �ð��� �ִϸ��̼��� 80% �Ϸ�� ����
        if (m_CurrentAnimationTime > 0.80f)
            m_Self.AnimationJobs.Enqueue(AniType.IDLE_0);
    }
}
