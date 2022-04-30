using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    [SerializeField] GameObject MonsterRoot;
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    public PoolSystem Pool { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // Init System 
        Pool = new PoolSystem();
        Pool.Init();
    }

    [ContextMenu("# Get Monsters")]
    void GetMonsters()
    {
        for (int i = 0; i < MonsterRoot.transform.childCount; i++)
        {
            var child = MonsterRoot.transform.GetChild(i);
            var monster = child.GetComponent<Character>();
            if (monster != null)
                Monsters.Add(monster);
        }
    }

    [ContextMenu("# Get Attached System")]
    void GetAttachedSystem()
    {
        
    }
}
