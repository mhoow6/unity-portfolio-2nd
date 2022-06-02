using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterspawnTrigger : AreaTrigger
{
    protected override void OnAwake()
    {
        m_AutoDisable = true;
        m_AutoWall = true;
    }

    protected override void OnAreaEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            var parent = StageManager.Instance.Areas.Find(a => a.Index == m_AreaIdx);
            if (parent != null)
                parent.InitSpawner();
        }
    }
}