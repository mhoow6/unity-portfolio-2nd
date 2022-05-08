using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    // �����Ϳ��� ���� ���� �־���� �ϴ� �͵�
    public int WorldIdx;
    public int StageIdx;
    public List<Area> Areas = new List<Area>();
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    public PoolSystem Pool { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // �ý��� Init
        Pool = new PoolSystem();
        Pool.Init();

        // �ΰ��ӿ� ���Ǵ� �͵� Init
        foreach (var area in Areas)
            area.Init();
    }

    [ContextMenu("# Get Attached System")]
    void GetAttachedSystem()
    {
        
    }
}
