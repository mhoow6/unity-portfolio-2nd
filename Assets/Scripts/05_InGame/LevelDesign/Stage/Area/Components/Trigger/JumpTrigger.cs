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
            
            var player = StageManager.Instance.Player;

            // Ʈ������ forward �������� ĳ���� ȸ��
            player.CurrentCharacter.transform.forward = transform.forward;

            // ����
            player.InputY();
            player.CurrentCharacter.AniSpeed = 1 / m_JumpTime;

            // ��Ʈ�� �Ұ���
            player.Moveable = false;
            player.Controlable = false;
            player.CurrentCharacter.Physics = false;

            // �ùķ��̼� ������ ����
            BeizerCurve curve = GetComponent<BeizerCurve>();
            curve.TargetObject = other.gameObject;

            // �ùķ��̼� ����
            curve.Simulate(m_JumpTime, () =>
            {
                player.Moveable = true;
                player.Controlable = true;
                player.CurrentCharacter.Physics = true;

                // ���� ��
                GameManager.InputSystem.PressYButton = false;
                player.AnimationJobs.Enqueue(AniType.IDLE_0);
                player.CurrentCharacter.AniSpeed = 1;

                player.CurrentCharacter.TryAttachToFloor();

                gameObject.SetActive(false);
            });
        }
    }
}
