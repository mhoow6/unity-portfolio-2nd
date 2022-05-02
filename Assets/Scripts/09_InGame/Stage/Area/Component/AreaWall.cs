using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaWall : AreaComponent
{
    void OnTriggerStay(Collider other)
    {
        // TODO: 캐릭터 반대방향으로 밀어내기
    }
}
