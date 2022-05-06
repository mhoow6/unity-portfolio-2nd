using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSpider : Monster
{
    public override ObjectCode Code => ObjectCode.CHAR_GreenSpider;

    protected override void OnDamaged(float updateHp)
    {
        Debug.Log($"{Name}(��)�� {updateHp}��ŭ�� �������� �޾ҽ��ϴ� �Ф�");
    }
}
