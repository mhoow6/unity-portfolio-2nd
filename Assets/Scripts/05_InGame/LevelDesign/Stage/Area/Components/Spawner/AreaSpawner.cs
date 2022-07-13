using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawner : AreaComponent
{
    [Header("# 기본 스포너 수동기입")]
    public int Priority;
    public Character SpawnPrefab;
    public bool NoRequireTargetSearch;

    [Tooltip("스폰트리거 작동시 처음에 나올 몬스터의 수")]
    public int FirstSpawnCount;

    [Tooltip("최종적으로 나올 처음에 나올 몬스터의 수")]
    public int TotalSpawnCount;
    [ReadOnly, SerializeField] int m_CurrentSpawnCount;
    [SerializeField] List<Transform> m_SpawnPositions = new List<Transform>();

    [HideInInspector] public List<Character> Monsters = new List<Character>();
    const float SPAWN_COOLTIME = 3f;

    private void Awake()
    {
        if (Priority <= 0)
        {
            Debug.LogError($"스포너의 Priority는 {Priority}가 될 수 없습니다. 최소한 1 이상의 값으로 설정하세요.");
            enabled = false;
            return;
        }

        if (TotalSpawnCount < FirstSpawnCount)
            FirstSpawnCount = TotalSpawnCount;
    }

    /// <summary> 스폰 지점들에서 몬스터들이 소환됩니다. </summary> ///
    public void SpawnMonsters()
    {
        if (m_CurrentSpawnCount == TotalSpawnCount)
            return;

        StartCoroutine(SpawnMonsterCoroutine(FirstSpawnCount));
    }

    /// <summary> 스폰 지점중 랜덤한 곳에서 몬스터가 소환됩니다. </summary> ///
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

            // 스폰을 더 해야되는 상황이면
            if (m_CurrentSpawnCount < TotalSpawnCount)
                StartCoroutine(SpawnMonsterCoroutine());
            else
            {
                if (StageManager.Instance.Monsters.Count > 0)
                    return;

                // 자기를 포함하는 Area 찾기
                var area = StageManager.Instance.Areas.Find(a => a.IsSpawnerIn(this));
                if (area != null)
                {
                    // 다음 스포너에서 몬스터 생성 시작
                    area.SpawnMonsterFromNewSpawner();
                }
            }
        }
    }
}
