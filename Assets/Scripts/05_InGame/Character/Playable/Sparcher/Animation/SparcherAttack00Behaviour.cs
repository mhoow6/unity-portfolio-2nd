using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SparcherAttack00Behaviour : SparcherBehaviour
{
    protected override AniType m_AniType => AniType.ATTACK_0;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        m_Player = StageManager.Instance.Player;
        m_Player.Controlable = false;

        var sparcher = m_Player.CurrentCharacter as Sparcher;
        var skillData = JsonManager.Instance.JsonDatas[Character.GetAttackIndex(sparcher.Code)] as SparcherBasicAttackData;

        // ���� Ÿ������ ���������� ���ݰŸ����� ���Ϳ��� ���� ȸ���Ѵ�.
        if (GameManager.Instance.AutoTargeting)
        {
            var ordered = StageManager.Instance.Monsters.OrderBy(m => Vector3.Distance(m.transform.position, sparcher.transform.position)).ToList();
            var find = ordered.Find(m => Vector3.Distance(m.transform.position, sparcher.transform.position) <= skillData.AutoTargetDetectRange);
            if (find != null)
            {
                sparcher.Target = find;
                sparcher.transform.LookAt(find.transform);
            }
        }
    }

    protected override void OnAnimationUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �����Ӱ� �ൿ�� �� �ִ� �ð��� �ִϸ��̼��� 50% �Ϸ�� ����
        if (m_CurrentAnimationTime > 0.5f)
        {
            if (m_InputSystem.PressXButton)
                m_Player.AnimationJobs.Enqueue(AniType.DASH_0);
            else
                m_Player.AnimationJobs.Enqueue(AniType.IDLE_0);
        }
    }

    protected override void OnAnimationExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player.Controlable = true;
    }
}
