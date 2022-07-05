using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),(typeof(Rigidbody)))]
public class Projectile : BaseObject, IPoolable
{
    public void SetData(Character owner, float damageScale)
    {
        m_Owner = owner;
        m_DamageScale = damageScale;
    }

    #region 투사체 발사
    public void ShootStraight(Vector3 direction, float moveSpeed, int lifeTime)
    {
        StartCoroutine(ShootStraightCoroutine(direction, moveSpeed, lifeTime));
    }

    public void ShootParabola(Vector3 startPosition, Vector3 endPosition, float height, float lifeTime)
    {
        StartCoroutine(ShootParabolaCoroutine(startPosition, endPosition, height, lifeTime));
    }
    #endregion

    #region 오브젝트 풀링
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }
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

    // -----------------------------------------------------------------------

    protected Character m_Owner;
    protected DamageType m_DamageType;

    #region 충돌 판정
    protected void OnTriggerEnter(Collider other)
    {
        // 지형 충돌시 풀에게 자기 반환
        if (other.gameObject.layer == GameManager.GameDevelopSettings.TerrainLayermask)
        {
            StageManager.Instance.PoolSystem.Release(this);
            return;
        }

        // 캐릭터와 충돌시
        var rhs = other.GetComponent<Character>();
        if (rhs != null && !other.CompareTag(m_Owner.tag))
        {
            var result = m_Owner.CalcuateDamage(rhs, m_DamageScale);

            // 실제 데미지
            rhs.Damaged(m_Owner, result.Damage, result.IsCrit);

            OnCollide(other);

            // 풀에게 자기 반환
            StageManager.Instance.PoolSystem.Release(this);
        }
    }
    protected virtual void OnCollide(Collider other) { }
    #endregion

    // -----------------------------------------------------------------------

    [SerializeField] Rigidbody m_RigidBody;
    [SerializeField] SphereCollider m_SphereCollider;

    float m_DamageScale;

    const float SHOOT_VELOCITY = 2f;

    #region 투사체 발사
    IEnumerator ShootStraightCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        float timer = 0f;
        m_RigidBody.isKinematic = false;
        m_RigidBody.useGravity = false;
        m_RigidBody.velocity = direction.normalized * moveSpeed * SHOOT_VELOCITY;
        while (timer < lifeTime)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        StageManager.Instance.PoolSystem.Release(this);
    }

    IEnumerator ShootParabolaCoroutine(Vector3 startPosition, Vector3 endPosition, float height, float lifeTime)
    {
        float timer = 0f;
        m_RigidBody.isKinematic = false;
        m_RigidBody.useGravity = false;
        while (timer < lifeTime)
        {
            timer += Time.fixedDeltaTime;

            m_RigidBody.position = MathParabola.Parabola(startPosition, endPosition, height, timer / lifeTime);

            yield return new WaitForFixedUpdate();
        }
        StageManager.Instance.PoolSystem.Release(this);
    }
    #endregion

    #region 오브젝트 풀링
    bool m_Poolable;
    #endregion
}

/// <summary> 투사체 궤적 타입 </summary> ///
public enum TrajectoryType
{
    /// <summary> 직선 </summary>
    Straight,
    /// <summary> 포물선 </summary>
    Parabola
}
