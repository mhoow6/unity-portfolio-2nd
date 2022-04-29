using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherWoodenArrow : Projectile
{
    public override ObjectCode Code => ObjectCode.PROJ_WoodenArrow;
    protected override void OnCollide(Collider other)
    {
        Debug.Log($"{other.name}�� �¾ҽ��ϴ�.");

        StageManager.Instance.Pool.Release(this);
    }
}
