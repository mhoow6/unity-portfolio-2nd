using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSpider : Character
{
    public override ObjectCode Code => ObjectCode.CHAR_GreenSpider;

    protected override void OnSpawn()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.Monsters.Add(this);
    }

    protected override void OnDead()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.Monsters.Remove(this);
    }
}
