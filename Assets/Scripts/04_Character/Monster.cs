using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    /// <summary> ���� Ŭ���� ������ ȣ�� </summary> ///
    protected virtual void OnMonsterSpawn() { }

    /// <summary> ���� Ŭ���� ����� ȣ�� </summary> ///
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
            // �������� �Ŵ����� ���� ����Ʈ���� ����
            manager.Monsters.Remove(this);

            // �ڽ��� �¾ �����ʰ� ������ �˷��ֱ�
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
