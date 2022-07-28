using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Playable
{
    public override ObjectCode Code => ObjectCode.CHAR_Knight;

    public override void OnXInput()
    {
        // 이동속도 증가 버프 (자기 자신만)
    }

    public override void OnBInput()
    {
        // 쉴드 이펙트와 함께 방어력 증가 버프 (캐릭터 교체시에도 적용)
    }
}
