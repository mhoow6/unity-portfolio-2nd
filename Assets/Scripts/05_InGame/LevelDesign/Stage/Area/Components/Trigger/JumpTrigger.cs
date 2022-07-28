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

            // 트리거의 forward 방향으로 캐릭터 회전
            player.CurrentCharacter.transform.forward = transform.forward;

            // 점프
            player.InputY();
            player.CurrentCharacter.AniSpeed = 1 / m_JumpTime;

            // 컨트롤 불가능
            player.Moveable = false;
            player.Controlable = false;
            player.CurrentCharacter.Physics = false;

            // 시뮬레이션 데이터 세팅
            BeizerCurve curve = GetComponent<BeizerCurve>();
            curve.TargetObject = other.gameObject;

            // 시뮬레이션 시작
            curve.Simulate(m_JumpTime, () =>
            {
                player.Moveable = true;
                player.Controlable = true;
                player.CurrentCharacter.Physics = true;

                // 점프 끝
                GameManager.InputSystem.PressYButton = false;
                player.AnimationJobs.Enqueue(AniType.IDLE_0);
                player.CurrentCharacter.AniSpeed = 1;

                player.CurrentCharacter.TryAttachToFloor();

                gameObject.SetActive(false);
            });
        }
    }
}
