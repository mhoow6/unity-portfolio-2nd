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

    public void Init()
    {
        Trigger.SetData(AreaIdx);
        m_Walls.ForEach(w =>
        {
            w.gameObject.SetActive(false);
            w.SetData(AreaIdx);
        });
    }
}
