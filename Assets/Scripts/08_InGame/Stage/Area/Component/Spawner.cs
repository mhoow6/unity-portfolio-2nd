using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Spawner : AreaComponent, IComparable<Spawner>
{
    public int Priority;
    public GameObject SpawnPrefab;
    public int SpawnCount;

    public List<Character> SpawnMonsters = new List<Character>();
    [SerializeField] List<Transform> m_SpawnPositions = new List<Transform>();

    public int CompareTo(Spawner other)
    {
        // lhs < rhs => 작은 순 정렬
        return other.Priority.CompareTo(Priority);
    }

    public void Spawn()
    {
        foreach (var pos in m_SpawnPositions)
        {
            // Start()호출
            Instantiate(SpawnPrefab, pos);

            var comp = SpawnPrefab.GetComponent<Character>();
            if (comp != null)
                SpawnMonsters.Add(comp);
        }
    }

    public bool NotifyDead(Character deadObj)
    {
        var find = SpawnMonsters.Find(m => m.Equals(deadObj));
        if (find != null)
        {
            SpawnMonsters.Remove(deadObj);
            return true;
        }
        return false;
    }
}
