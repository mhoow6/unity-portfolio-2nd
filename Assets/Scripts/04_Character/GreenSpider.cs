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

    protected override void OnDamaged(float updateHp)
    {
        Debug.Log($"{Name}(이)가 {updateHp}만큼의 데미지를 받았습니다 ㅠㅠ");
    }

    protected override void OnDead()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.Monsters.Remove(this);
    }
}
