using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    /// <summary> 몬스터 클래스 스폰시 호출 </summary> ///
    protected virtual void OnMonsterSpawn() { }

    /// <summary> 몬스터 클래스 사망시 호출 </summary> ///
    protected virtual void OnMonsterDead() { }

    protected override void OnSpawn()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.Monsters.Add(this);

        OnMonsterSpawn();
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
        
        OnMonsterDead();
    }
}
