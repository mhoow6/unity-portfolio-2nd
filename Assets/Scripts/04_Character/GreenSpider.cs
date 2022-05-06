using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSpider : Monster
{
    public override ObjectCode Code => ObjectCode.CHAR_GreenSpider;

    protected override void OnDamaged(float updateHp)
    {
        Debug.Log($"{Name}(이)가 {updateHp}만큼의 데미지를 받았습니다 ㅠㅠ");
    }
}
