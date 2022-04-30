using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherWoodenArrow : Projectile
{
    public override ObjectCode Code => ObjectCode.PROJ_WoodenArrow;
    protected override void OnCollide(Collider other)
    {
        var rhs = other.GetComponent<Character>();
        if (rhs != null)
        {
            var result = m_Owner.CalcuateDamage(rhs);

            // 실제 데미지
            rhs.Hp -= result.Item1;

            // 데미지 텍스트
            var damageText = GameManager.Instance.UISystem.Pool.Load<FloatingDamageText>($"06_UI/FloatingDamage");
            var floatingStartPoint = GameManager.Instance.MainCam.WorldToScreenPoint(rhs.Head.position);
            damageText.SetData(result.Item1, result.Item2, floatingStartPoint, rhs.Head.position);

            damageText.StartFloating();
        }

        StageManager.Instance.Pool.Release(this);
    }
}
