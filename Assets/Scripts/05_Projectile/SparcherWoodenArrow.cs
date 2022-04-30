using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherWoodenArrow : Projectile
{
    public override ObjectCode Code => ObjectCode.PROJ_WoodenArrow;
    protected override void OnCollide(Collider other)
    {
        Debug.Log($"{other.name}가 맞았습니다.");
        var rhs = other.GetComponent<Character>();
        if (rhs != null)
        {
            var damageText = GameManager.Instance.UISystem.Pool.Load<FloatingDamageText>($"06_UI/FloatingDamage");

            var result = m_Owner.CalcuateDamage(rhs);
            var floatingStartPoint = GameManager.Instance.MainCam.WorldToScreenPoint(rhs.Head.position);
            damageText.SetData((int)result.Item1, result.Item2, floatingStartPoint, rhs.Head.position);

            damageText.StartFloating();
        }

        StageManager.Instance.Pool.Release(this);
    }
}
