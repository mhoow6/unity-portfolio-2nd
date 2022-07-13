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

    #region ������
    /// <summary> spawner�� ���⿡ ���������� True </summary> /// 
    public bool IsSpawnerIn(AreaSpawner spawner)
    {
        return m_Spawners.Find(s => s.Equals(spawner)) != null ? true : false;
    }

    /// <summary> �������� belongTo�� �����ִ� �����ʸ� ã���ϴ�. </summary> /// 

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

    /// <summary> ���ο� ������ �۵��� �����մϴ�. </summary> /// 
    public void SpawnMonsterFromNewSpawner()
    {
        if (m_CurrentSpawnerCount < m_Spawners.Count)
        {
            m_Spawners[m_CurrentSpawnerCount].SpawnMonsters();
            m_CurrentSpawnerCount++;
        }
        else
        {
            // ��� �����ʿ��� ������ �Ϸ�� ��� �� ����
            m_Walls.ForEach(w => w.gameObject.SetActive(false));

            // ������ �����ʰ� Ŭ���� �����̶��?
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
        // 0: ����, 1: ������ ũ��, -1: �������� ũ��
        if (this.AreaIdx > other.AreaIdx)
            return 1;

        if (this.AreaIdx < other.AreaIdx)
            return -1;

        return 0;
    }
}
