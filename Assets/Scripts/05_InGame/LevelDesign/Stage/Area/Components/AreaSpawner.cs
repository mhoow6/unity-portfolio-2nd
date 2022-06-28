using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawner : AreaComponent
{
    [Header("# ��������")]
    public int Priority;
    public Character SpawnPrefab;

    [Tooltip("����Ʈ���� �۵��� ó���� ���� ������ ��")]
    public int FirstSpawnCount;

    [Tooltip("���������� ���� ó���� ���� ������ ��")]
    public int TotalSpawnCount;
    [ReadOnly, SerializeField] int m_CurrentSpawnCount;
    [SerializeField] List<Transform> m_SpawnPositions = new List<Transform>();

    [HideInInspector] public List<Character> Monsters = new List<Character>();
    const float SPAWN_COOLTIME = 3f;

    private void Awake()
    {
        if (TotalSpawnCount < FirstSpawnCount)
            FirstSpawnCount = TotalSpawnCount;
    }

    /// <summary> ���� �����鿡�� ���͵��� ��ȯ�˴ϴ�. </summary> ///
    public void SpawnMonsters()
    {
        StartCoroutine(SpawnMonsterCoroutine(FirstSpawnCount));
    }

    /// <summary> ���� ������ ������ ������ ���Ͱ� ��ȯ�˴ϴ�. </summary> ///
    IEnumerator SpawnMonsterCoroutine(int count = 1)
    {
        yield return new WaitForSeconds(SPAWN_COOLTIME);

        while (count != 0)
        {
            int random = UnityEngine.Random.Range(0, m_SpawnPositions.Count);
            var pos = m_SpawnPositions[random];

            var mob = Instantiate(SpawnPrefab, pos);
            var comp = mob.GetComponent<Character>();
            if (comp != null)
            {
                comp.Spawn();
                Monsters.Add(comp);
            }

            m_CurrentSpawnCount++;

            count--;
        }
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
