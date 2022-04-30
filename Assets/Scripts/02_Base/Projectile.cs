using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),(typeof(Rigidbody)))]
public class Projectile : BaseObject, IPoolable
{
    protected Character m_Owner;

    [SerializeField] Rigidbody m_RigidBody;
    [SerializeField] SphereCollider m_SphereCollider;
    bool m_Poolable;

    const float m_SHOOT_VELOCITY = 2f;

    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }

    protected void OnTriggerEnter(Collider other)
    {
        OnCollide(other);
    }

    protected virtual void OnCollide(Collider other) { }

    public void SetData(Character owner)
    {
        m_Owner = owner;
    }

    #region ����ü �߻�
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
        m_RigidBody.velocity = direction.normalized * moveSpeed * m_SHOOT_VELOCITY;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        StageManager.Instance.Pool.Release(this);
    }

    IEnumerator ShootParabolaCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        yield return null;
    }
    #endregion

    #region ������Ʈ Ǯ��
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

/// <summary> ����ü ���� Ÿ�� </summary> ///
public enum TrajectoryType
{
    /// <summary> ���� </summary>
    Straight,
    /// <summary> ������ </summary>
    [Obsolete]
    Parabola
}
