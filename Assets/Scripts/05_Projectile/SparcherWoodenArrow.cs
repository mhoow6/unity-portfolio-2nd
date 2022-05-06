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
            rhs.Damaged(result.Item1, DamageType.Normal);

            // 데미지 텍스트
            var damageText = GameManager.Instance.UISystem.Pool.Load<FloatingDamageText>($"{GameManager.Instance.Config.UIResourcePath}/FloatingDamage");
            var floatingStartPoint = GameManager.Instance.MainCam.WorldToScreenPoint(rhs.Head.position);
            damageText.SetData(result.Item1, result.Item2, floatingStartPoint, rhs.Head.position);

            damageText.StartFloating();

            StageManager.Instance.Pool.Release(this);
        }
    }
}
