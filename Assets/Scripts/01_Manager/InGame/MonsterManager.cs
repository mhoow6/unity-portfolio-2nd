using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoSingleton<MonsterManager>
{
    [SerializeField] GameObject MonsterRoot;
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    [ContextMenu("# Get Monsters")]
    void GetAttachedSystem()
    {
        for (int i = 0; i < MonsterRoot.transform.childCount; i++)
        {
            var child = MonsterRoot.transform.GetChild(i);
            var monster = child.GetComponent<Character>();
            if (monster != null)
                Monsters.Add(monster);
        }
    }
}
