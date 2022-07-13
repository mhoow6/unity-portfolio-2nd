using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawner : AreaComponent
{
    [Header("# �⺻ ������ ��������")]
    public int Priority;
    public Character SpawnPrefab;
    public bool NoRequireTargetSearch;

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
        if (Priority <= 0)
        {
            Debug.LogError($"�������� Priority�� {Priority}�� �� �� �����ϴ�. �ּ��� 1 �̻��� ������ �����ϼ���.");
            enabled = false;
            return;
        }

        if (TotalSpawnCount < FirstSpawnCount)
            FirstSpawnCount = TotalSpawnCount;
    }

    /// <summary> ���� �����鿡�� ���͵��� ��ȯ�˴ϴ�. </summary> ///
    public void SpawnMonsters()
    {
        if (m_CurrentSpawnCount == TotalSpawnCount)
            return;

        StartCoroutine(SpawnMonsterCoroutine(FirstSpawnCount));
    }

    /// <summary> ���� ������ ������ ������ ���Ͱ� ��ȯ�˴ϴ�. </summary> ///
    IEnumerator SpawnMonsterCoroutine(int count = 1)
    {
        yield return new WaitForSeconds(SPAWN_COOLTIME);
        Debug.Log($"{m_CurrentSpawnCount} -> {m_CurrentSpawnCount + count}");
        if (m_CurrentSpawnCount >= TotalSpawnCount)
            yield break;

        while (count != 0)
        {
            int posIndex = 0;
            posIndex = m_CurrentSpawnCount % m_SpawnPositions.Count;

            Transform pos = m_SpawnPositions[posIndex];

            var mob = Instantiate(SpawnPrefab, pos);
            var comp = mob.GetComponent<Character>();
            if (comp != null)
            {
                comp.Spawn();
                comp.name = $"{comp.name.EraseBracketInName()} ({m_CurrentSpawnCount+1})";

                Monsters.Add(comp);
            }

            if (NoRequireTargetSearch)
                comp.Target = StageManager.Instance.Player.CurrentCharacter;

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
                if (StageManager.Instance.Monsters.Count > 0)
                    return;

                // �ڱ⸦ �����ϴ� Area ã��
                var area = StageManager.Instance.Areas.Find(a => a.IsSpawnerIn(this));
                if (area != null)
                {
                    // ���� �����ʿ��� ���� ���� ����
                    area.SpawnMonsterFromNewSpawner();
                }
            }
        }
    }
}
