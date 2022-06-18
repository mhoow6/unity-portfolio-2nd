using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawner : AreaComponent
{
    public int Priority;
    public Character SpawnPrefab;
    public int TotalSpawnCount;
    [ReadOnly, SerializeField] int m_CurrentSpawnCount;

    [HideInInspector] public List<Character> Monsters = new List<Character>();
    [SerializeField] List<Transform> m_SpawnPositions = new List<Transform>();

    const float m_SPAWN_COOLTIME = 3f;

    /// <summary> ���� �����鿡�� ���͵��� ��ȯ�˴ϴ�. </summary> ///
    public void SpawnMonsters()
    {
        foreach (var pos in m_SpawnPositions)
        {
            var mob = Instantiate(SpawnPrefab, pos);
            if (mob != null)
            {
                mob.Spawn();
                Monsters.Add(mob);
            }
            
            m_CurrentSpawnCount++;
        }
    }

    /// <summary> ���� ������ ������ ������ ���Ͱ� ��ȯ�˴ϴ�. </summary> ///
    IEnumerator SpawnMonsterCoroutine()
    {
        int random = UnityEngine.Random.Range(0, m_SpawnPositions.Count);
        var pos = m_SpawnPositions[random];

        yield return new WaitForSeconds(m_SPAWN_COOLTIME);

        var mob = Instantiate(SpawnPrefab, pos);
        var comp = mob.GetComponent<Character>();
        if (comp != null)
        {
            comp.Spawn();
            Monsters.Add(comp);
        }
            
        m_CurrentSpawnCount++;
    }

    public void NotifyDead(Character deadObj)
    {
        var find = Monsters.Find(m => m.Equals(deadObj));
        if (find != null)
        {
            Monsters.Remove(deadObj);

            // ������ �� �ؾߵǴ� ��Ȳ�̸�
            if (m_CurrentSpawnCount < TotalSpawnCount)
                StartCoroutine(SpawnMonsterCoroutine());
            else
            {
                // �ڱ⸦ �����ϴ� Area ã��
                var area = StageManager.Instance.Areas.Find(a => a.IsSpawnerIn(this));
                if (area != null)
                {
                    // ���� �����ʿ��� ���� ���� ����
                    area.InitSpawner();
                }
            }
        }
    }
}
