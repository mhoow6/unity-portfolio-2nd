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
    public bool ComponentActive
    {
        set
        {
            m_Walls.ForEach(w => w.gameObject.SetActive(value));
            m_Triggers.ForEach(t => t.gameObject.SetActive(value));
            m_Spawners.ForEach(s => s.gameObject.SetActive(value));
        }
    }

    [SerializeField, ReadOnly] int m_CurrentSpawnerPriority = 0;

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
    public void InitSpawner()
    {
        if (m_CurrentSpawnerPriority < m_Spawners.Count)
        {
            m_Spawners[m_CurrentSpawnerPriority].SpawnMonsters();
            m_CurrentSpawnerPriority++;
        }
        else
        {
            // ��� �����ʿ��� ������ �Ϸ�� ��� �� ����
            m_Walls.ForEach(w => w.gameObject.SetActive(false));
        }
    }
    #endregion
}
