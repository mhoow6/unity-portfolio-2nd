using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);

    protected override void OnSpawn()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.Monsters.Add(this);

        gameObject.tag = "Monster";
        gameObject.layer = GameManager.GameDevelopSettings.BaseObjectLayermask;
    }

    protected override void OnLive()
    {
        // ���⿡�� �����Ӻ��� �ִϸ��̼��� �ϳ��� �޾ƿ����� ó��
        if (AnimationJobs.Count > 0)
            Animator.SetInteger(ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());
    }

    protected override void OnDamaged(Character attacker, int damage, DamageType damageType)
    {
        Debug.Log($"{attacker.Name}���� {Name}��(��) {damage}��ŭ�� �������� �Ծ����ϴ�.");
    }

    protected override void OnDead(Character attacker, int damage, DamageType damageType)
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
        AnimationJobs.Enqueue(AniType.DEAD_0);

        // �÷��̾ ���� �׿����� �� óġ Ƚ�� ����
        StageManager.Instance.MissionSystem.ReportAll(QuestType.KILL_ENEMY);
    }
}
