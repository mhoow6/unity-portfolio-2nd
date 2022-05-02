using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    // �����Ϳ��� ���� ���� �־���� �ϴ� �͵�
    public int WorldIdx;
    public int StageIdx;
    public List<Area> Areas = new List<Area>();
    [SerializeField] GameObject m_MonsterRoot;

    [ReadOnly] public List<Character> Monsters = new List<Character>();

    public PoolSystem Pool { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // Init System 
        Pool = new PoolSystem();
        Pool.Init();
    }

    private void Start()
    {
        // Ʈ���ſ� ��������
        foreach (var area in Areas)
            area.Trigger.SetData(area.Index);
    }

    [ContextMenu("# Get Monsters")]
    void GetMonsters()
    {
        for (int i = 0; i < m_MonsterRoot.transform.childCount; i++)
        {
            var child = m_MonsterRoot.transform.GetChild(i);
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
