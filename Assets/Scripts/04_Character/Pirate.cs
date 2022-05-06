using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirate : Monster
{
    public override ObjectCode Code => ObjectCode.CHAR_Pirate;

    protected override void OnMonsterDead()
    {
        Destroy(gameObject);
    }
}
