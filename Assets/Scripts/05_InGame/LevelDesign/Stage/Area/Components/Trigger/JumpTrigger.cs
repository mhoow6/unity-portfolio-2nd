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
            // 컨트롤 불가능
            var player = StageManager.Instance.Player;
            player.Moveable = false;
            player.Controlable = false;
            player.CurrentCharacter.Physics = false;

            // 트리거의 forward 방향으로 캐릭터 회전
            player.CurrentCharacter.transform.forward = transform.forward;

            // 점프 애니메이션 강제로
            player.AnimationJobs.Enqueue(AniType.JUMP_0);
            player.CurrentCharacter.AniSpeed = 1 / m_JumpTime;

            // 시뮬레이션 데이터 세팅
            BeizerCurve curve = GetComponent<BeizerCurve>();
            curve.TargetObject = other.gameObject;

            // 시뮬레이션 시작
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
