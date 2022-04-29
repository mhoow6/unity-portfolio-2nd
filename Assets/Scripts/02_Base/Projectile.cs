using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),(typeof(Rigidbody)))]
public class Projectile : BaseObject, IPoolable
{
    [SerializeField] Rigidbody m_RigidBody;
    [SerializeField] SphereCollider m_SphereCollider;

    const float m_SHOOT_VELOCITY = 2f;
    bool m_Poolable;

    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }

    protected void OnTriggerEnter(Collider other)
    {
        OnCollide(other);
    }

    protected virtual void OnCollide(Collider other) { }

    #region 투사체 발사
    public void Shoot(GameObject shooter, Vector3 direction, TrajectoryType trajectoryType, float moveSpeed, int lifeTime)
    {
        switch (trajectoryType)
        {
            case TrajectoryType.Straight:
                StartCoroutine(ShootStraightCoroutine(shooter, direction, moveSpeed, lifeTime));
                break;
            default:
                break;
        }
    }

    IEnumerator ShootStraightCoroutine(GameObject shooter, Vector3 direction, float moveSpeed, int lifeTime)
    {
        float timer = 0f;
        m_RigidBody.isKinematic = false;
        m_RigidBody.useGravity = false;
        m_RigidBody.velocity = direction.normalized * moveSpeed * m_SHOOT_VELOCITY;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        StageManager.Instance.Pool.Release(this);
    }

    IEnumerator ShootParabolaCoroutine(GameObject shooter, Vector3 direction, float moveSpeed, int lifeTime)
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
