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
    [SerializeField, ReadOnly] int m_CurrentSpawnerPriority = 0;

    [SerializeField] AreaTrigger Trigger;
    [SerializeField] List<AreaWall> m_Walls = new List<AreaWall>();
    [SerializeField] List<Spawner> m_Spawners = new List<Spawner>();
    
    bool m_Init;

    public void Init()
    {
        if (m_Init)
            return;

        Trigger.SetData(AreaIdx);

        m_Walls.ForEach(w =>
        {
            w.gameObject.SetActive(false);
            w.SetData(AreaIdx);
        });

        m_Spawners.ForEach(s =>
        {
            s.SetData(AreaIdx);
        });
        m_Spawners.Sort((lhs, rhs) => { return lhs.Priority.CompareTo(rhs.Priority); });

        m_Init = true;
    }

    #region 스포너
    /// <summary> spawner가 여기에 속해있으면 True </summary> /// 
    public bool IsSpawnerIn(Spawner spawner)
    {
        return m_Spawners.Find(s => s.Equals(spawner)) != null ? true : false;
    }

    /// <summary> 스포너중 belongTo가 속해있는 스포너를 찾습니다. </summary> /// 

    public Spawner FindSpawner(Character belongTo)
    {
        Spawner res = null;
        foreach (var spawner in m_Spawners)
        {
            foreach (var mob in spawner.Monsters)
            {
                if (mob.Equals(belongTo))
                {
                    res = spawner;
                    return res;
                }
            }
        }
        return null;
    }

    public void InitSpawner()
    {
        if (m_CurrentSpawnerPriority < m_Spawners.Count)
        {
            m_Spawners[m_CurrentSpawnerPriority].SpawnMonsters();
            m_CurrentSpawnerPriority++;
        }
        else
        {
            // 모든 스포너에서 스폰이 완료된 경우 벽 해제
            m_Walls.ForEach(w => w.gameObject.SetActive(false));
        }
    }
    #endregion
}
