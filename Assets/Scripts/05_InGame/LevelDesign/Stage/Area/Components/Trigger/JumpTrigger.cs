using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : AreaTrigger
{
    [Range(1, 5), SerializeField]
    float m_JumpTime = 1;

    protected override void OnAwake()
    {
        m_AutoDisable = false;
        m_AutoWall = false;
    }

    protected override void OnAreaEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ��Ʈ�� �Ұ���
            var player = StageManager.Instance.Player;
            player.Moveable = false;
            player.Controlable = false;
            player.CurrentCharacter.Physics = false;

            // Ʈ������ forward �������� ĳ���� ȸ��
            player.CurrentCharacter.transform.forward = transform.forward;

            // ���� �ִϸ��̼� ������
            player.AnimationJobs.Enqueue(AniType.JUMP_0);
            player.CurrentCharacter.AniSpeed = 1 / m_JumpTime;

            // �ùķ��̼� ������ ����
            BeizerCurve curve = GetComponent<BeizerCurve>();
            curve.TargetObject = other.gameObject;

            // �ùķ��̼� ����
            curve.Simulate(m_JumpTime, () =>
            {
                player.Moveable = true;
                player.Controlable = true;
                player.CurrentCharacter.Physics = true;

                player.AnimationJobs.Enqueue(AniType.IDLE_0);
                player.CurrentCharacter.AniSpeed = 1;

                player.CurrentCharacter.TryAttachToFloor();

                gameObject.SetActive(false);
            });
        }
    }
}
