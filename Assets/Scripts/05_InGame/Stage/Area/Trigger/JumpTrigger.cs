using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : AreaTrigger
{
    [Range(1, 5), SerializeField]
    const float JUMP_TIME = 1;

    private void Awake()
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
            player.Controlable = false;
            player.CurrentCharacter.Physic = false;

            // Ʈ������ forward �������� ĳ���� ȸ��
            player.CurrentCharacter.transform.forward = transform.forward;

            // ���� �ִϸ��̼�
            player.AnimationJobs.Enqueue(AniType.JUMP_0);
            player.CurrentCharacter.AniSpeed = 1 / JUMP_TIME;

            // �ùķ��̼� ������ ����
            BeizerCurve curve = GetComponent<BeizerCurve>();
            curve.TargetObject = other.gameObject;
            // �÷��̾ �ִ� x��ǥ�������� ������ �õ��ؾ��ϱ� ������ ������ ����� x��ǥ�鸸 ����
            curve.P1 = new Vector3(other.transform.position.x, curve.P1.y, curve.P1.z);
            curve.P2 = new Vector3(other.transform.position.x, curve.P2.y, curve.P2.z);
            curve.P3 = new Vector3(other.transform.position.x, curve.P3.y, curve.P3.z);
            curve.P4 = new Vector3(other.transform.position.x, curve.P4.y, curve.P4.z);

            // �ùķ��̼� ����
            curve.Simulate(JUMP_TIME, () =>
            {
                player.Controlable = true;
                player.CurrentCharacter.Physic = true;

                player.AnimationJobs.Enqueue(AniType.IDLE_0);
                player.CurrentCharacter.AniSpeed = 1;

                player.CurrentCharacter.TryAttachToFloor();

                gameObject.SetActive(false);
            });
        }
    }
}
