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
    public bool TriggerActive
    {
        set
        {
            m_Triggers.ForEach((t) => t.gameObject.SetActive(value));
        }
    }

    [SerializeField, ReadOnly] int m_CurrentSpawnerPriority = 0;

    [SerializeField] List<AreaTrigger> m_Triggers = new List<AreaTrigger>();
    [SerializeField] List<AreaWall> m_Walls = new List<AreaWall>();
    [SerializeField] List<Spawner> m_Spawners = new List<Spawner>();
    
    bool m_Init;

    [Header("# 개발자 옵션"), SerializeField]
    bool m_AreaActive = true;

    public void Init()
    {
        if (!m_AreaActive)
        {
            m_Triggers.ForEach(e => e.gameObject.SetActive(false));
            m_Walls.ForEach(e => e.gameObject.SetActive(false));
            m_Spawners.ForEach(e => e.gameObject.SetActive(false));
            return;
        }

        if (m_Init)
            return;

        m_Triggers.ForEach(t =>
        {
            t.gameObject.SetActive(false);
            t.SetData(AreaIdx);
        });

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

    /// <summary> 새로운 스포너 작동을 시작합니다. </summary> /// 
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
