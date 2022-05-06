using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterspawnTrigger : AreaTrigger
{
    protected override void OnAreaEnter(Collider other)
    {
        var parent = StageManager.Instance.Areas.Find(a => a.Index == m_AreaIdx);
        if (parent != null)
            parent.StartSpawning();
    }
}
