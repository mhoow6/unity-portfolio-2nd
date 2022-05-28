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
            // 컨트롤 불가능
            var player = StageManager.Instance.Player;
            player.Controlable = false;
            player.CurrentCharacter.Physic = false;

            // 트리거의 forward 방향으로 캐릭터 회전
            player.CurrentCharacter.transform.forward = transform.forward;

            // 점프 애니메이션
            player.AnimationJobs.Enqueue(AniType.JUMP_0);
            player.CurrentCharacter.AniSpeed = 1 / JUMP_TIME;

            // 시뮬레이션 데이터 세팅
            BeizerCurve curve = GetComponent<BeizerCurve>();
            curve.TargetObject = other.gameObject;
            // 플레이어가 있는 x좌표에서부터 점프를 시도해야하기 때문에 베지어 곡선들의 x좌표들만 변경
            curve.P1 = new Vector3(other.transform.position.x, curve.P1.y, curve.P1.z);
            curve.P2 = new Vector3(other.transform.position.x, curve.P2.y, curve.P2.z);
            curve.P3 = new Vector3(other.transform.position.x, curve.P3.y, curve.P3.z);
            curve.P4 = new Vector3(other.transform.position.x, curve.P4.y, curve.P4.z);

            // 시뮬레이션 시작
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
