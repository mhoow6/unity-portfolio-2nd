using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    public FixedQueue<AniType> Commands { get; private set; } = new FixedQueue<AniType>(1);

    protected override void OnSpawn()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.Monsters.Add(this);

        gameObject.tag = "Monster";
    }

    protected override void OnLive()
    {
        // 여기에서 프레임별로 애니메이션을 하나씩 받아오도록 처리
        if (Commands.Count > 0)
            Animator.SetInteger(ANITYPE_HASHCODE, (int)Commands.Dequeue());
    }

    protected override void OnDamaged(Character attacker, float updateHp)
    {
        Debug.Log($"{attacker.Name}에게 {Name}이(가) {updateHp}만큼의 데미지를 입었습니다.");
    }

    protected override void OnDead()
    {
        var manager = StageManager.Instance;
        if (manager != null)
        {
            // 스테이지 매니저의 몬스터 리스트에서 삭제
            manager.Monsters.Remove(this);

            // 자신이 태어난 스포너가 있으면 알려주기
            Spawner spawner = null;
            foreach (var area in manager.Areas)
            {
                spawner = area.FindSpawner(this);
                if (spawner != null)
                    break;
            }
            if (spawner != null)
                spawner.NotifyDead(this);
        }

        // 죽는 애니메이션
        Commands.Enqueue(AniType.DEAD_0);
    }
}
