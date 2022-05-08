using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    // 에디터에서 직접 값을 넣어줘야 하는 것들
    public int WorldIdx;
    public int StageIdx;
    public List<Area> Areas = new List<Area>();
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    public PoolSystem Pool { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // 시스템 Init
        Pool = new PoolSystem();
        Pool.Init();

        // 인게임에 사용되는 것들 Init
        foreach (var area in Areas)
            area.Init();
    }

    [ContextMenu("# Get Attached System")]
    void GetAttachedSystem()
    {
        
    }
}
