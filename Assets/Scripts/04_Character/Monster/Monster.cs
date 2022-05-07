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
        // ���⿡�� �����Ӻ��� �ִϸ��̼��� �ϳ��� �޾ƿ����� ó��
        if (Commands.Count > 0)
            Animator.SetInteger(ANITYPE_HASHCODE, (int)Commands.Dequeue());
    }

    protected override void OnDamaged(Character attacker, float updateHp)
    {
        Debug.Log($"{attacker.Name}���� {Name}��(��) {updateHp}��ŭ�� �������� �Ծ����ϴ�.");
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

        // �״� �ִϸ��̼�
        Commands.Enqueue(AniType.DEAD_0);
    }
}
