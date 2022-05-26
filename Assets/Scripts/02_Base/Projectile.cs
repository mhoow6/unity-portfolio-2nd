using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),(typeof(Rigidbody)))]
public class Projectile : BaseObject, IPoolable
{
    protected Character m_Owner;
    protected DamageType m_DamageType;

    [SerializeField] Rigidbody m_RigidBody;
    [SerializeField] SphereCollider m_SphereCollider;
    bool m_Poolable;

    const float SHOOT_VELOCITY = 2f;

    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }

    public void SetData(Character owner, DamageType damageType)
    {
        m_Owner = owner;
        m_DamageType = damageType;
    }

    #region 충돌 판정
    protected void OnTriggerEnter(Collider other)
    {
        var rhs = other.GetComponent<Character>();
        if (rhs != null && !other.CompareTag(m_Owner.tag))
        {
            var result = m_Owner.CalcuateDamage(rhs);

            // 실제 데미지
            rhs.Damaged(m_Owner, result.Item1, DamageType.Normal);

            // 데미지 텍스트
            var damageText = GameManager.UISystem.Pool.Load<FloatingDamageText>($"{GameManager.GameDevelopSettings.UIResourcePath}/InGame/FloatingDamage");
            var floatingStartPoint = GameManager.MainCam.WorldToScreenPoint(rhs.Head.position);
            damageText.SetData(result.Item1, result.Item2, floatingStartPoint, rhs.Head.position);
            damageText.StartFloating();

            OnCollide(other);

            // 풀에게 자기 반환
            StageManager.PoolSystem.Release(this);
        }
    }

    protected virtual void OnCollide(Collider other) { }
    #endregion

    #region 투사체 발사
    public void Shoot(Vector3 direction, TrajectoryType trajectoryType, float moveSpeed, int lifeTime)
    {
        switch (trajectoryType)
        {
            case TrajectoryType.Straight:
                StartCoroutine(ShootStraightCoroutine(direction, moveSpeed, lifeTime));
                break;
            default:
                break;
        }
    }

    IEnumerator ShootStraightCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        float timer = 0f;
        m_RigidBody.isKinematic = false;
        m_RigidBody.useGravity = false;
        m_RigidBody.velocity = direction.normalized * moveSpeed * SHOOT_VELOCITY;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        StageManager.PoolSystem.Release(this);
    }

    IEnumerator ShootParabolaCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        yield return null;
    }
    #endregion

    #region 오브젝트 풀링
    public void OnLoad()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    #endregion
}

/// <summary> 투사체 궤적 타입 </summary> ///
public enum TrajectoryType
{
    /// <summary> 직선 </summary>
    Straight,
    /// <summary> 포물선 </summary>
    [Obsolete]
    Parabola
}
