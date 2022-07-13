using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Area : MonoBehaviour, IAlarmReactable, IComparable<Area>
{
    public int Index => AreaIdx;
    [SerializeField] int AreaIdx;
    public bool Wall
    {
        set
        {
            if (value)
                m_Walls.ForEach(element => element.gameObject.SetActive(true));
            else
                m_Walls.ForEach(element => element.gameObject.SetActive(false));
        }
    }

    public bool Trigger
    {
        set
        {
            if (value)
                m_Triggers.ForEach(element => element.gameObject.SetActive(true));
            else
                m_Triggers.ForEach(element => element.gameObject.SetActive(false));
        }
    }

    public bool Spawner
    {
        set
        {
            if (value)
                m_Spawners.ForEach(element => element.gameObject.SetActive(true));
            else
                m_Spawners.ForEach(element => element.gameObject.SetActive(false));
        }
    }

    [SerializeField, ReadOnly] int m_CurrentSpawnerCount;

    [SerializeField] List<AreaTrigger> m_Triggers = new List<AreaTrigger>();
    [SerializeField] List<AreaWall> m_Walls = new List<AreaWall>();
    [SerializeField] List<AreaSpawner> m_Spawners = new List<AreaSpawner>();
    
    bool m_Init;

    public void Init()
    {
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
    public bool IsSpawnerIn(AreaSpawner spawner)
    {
        return m_Spawners.Find(s => s.Equals(spawner)) != null ? true : false;
    }

    /// <summary> 스포너중 belongTo가 속해있는 스포너를 찾습니다. </summary> /// 

    public AreaSpawner FindSpawner(Character belongTo)
    {
        AreaSpawner res = null;
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
    public void SpawnMonsterFromNewSpawner()
    {
        if (m_CurrentSpawnerCount < m_Spawners.Count)
        {
            m_Spawners[m_CurrentSpawnerCount].SpawnMonsters();
            m_CurrentSpawnerCount++;
        }
        else
        {
            // 모든 스포너에서 스폰이 완료된 경우 벽 해제
            m_Walls.ForEach(w => w.gameObject.SetActive(false));

            // 마지막 스포너가 클리어 조건이라면?
            var clearSpawner = m_Spawners[m_Spawners.Count - 1] as IStageClearable;
            if (clearSpawner != null)
                clearSpawner.ClearAction();
        }
    }
    #endregion

    public void React(AlarmEvent alarmEvent)
    {
        switch (alarmEvent)
        {
            case AlarmEvent.NONE:
                break;
            case AlarmEvent.SpawnBoss:
                var bossSpawner = m_Spawners.Find(spawner => spawner is BossSpawner);
                if (bossSpawner != null)
                {
                    SpawnMonsterFromNewSpawner();
                }
                break;
            default:
                break;
        }
    }

    public int CompareTo(Area other)
    {
        // 0: 같다, 1: 왼쪽이 크다, -1: 오른쪽이 크다
        if (this.AreaIdx > other.AreaIdx)
            return 1;

        if (this.AreaIdx < other.AreaIdx)
            return -1;

        return 0;
    }
}
