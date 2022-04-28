using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherWoodenArrow : Projectile
{
    public override ObjectCode Code => ObjectCode.PROJ_WoodenArrow;
    protected override void OnCollide(Collider other)
    {
        Debug.Log($"{other.name}가 맞았습니다.");

        // TODO: 오브젝트 풀링
        Destroy(gameObject);
    }
}
