using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public int Index => AreaIdx;
    [SerializeField] int AreaIdx;
    public bool Wall
    {
        set
        {
            if (value)
                m_Walls.ForEach(w => w.gameObject.SetActive(true));
            else
                m_Walls.ForEach(w => w.gameObject.SetActive(false));
        }
    }

    [SerializeField] AreaTrigger Trigger;
    [SerializeField] List<AreaWall> m_Walls = new List<AreaWall>();
    [SerializeField] List<Spawner> m_Spawners = new List<Spawner>();
    PriorityQueue<Spawner> m_SpawnQueue = new PriorityQueue<Spawner>();

    public void Init()
    {
        Trigger.SetData(AreaIdx);

        m_Walls.ForEach(w =>
        {
            w.gameObject.SetActive(false);
            w.SetData(AreaIdx);
        });

        m_Spawners.ForEach(s =>
        {
            s.SetData(AreaIdx);
            m_SpawnQueue.Push(s);
        });

    }

    /// <summary> 스포너중 belongTo가 속해있는 스포너를 찾습니다. </summary> /// 

    public Spawner FindSpawner(Character belongTo)
    {
        var res = m_Spawners.Find(s => s.SpawnMonsters.Find(m => m.Equals(belongTo)));
        return res;
    }

    public void StartSpawning()
    {
        m_SpawnQueue.Pop().Spawn();
    }
}
