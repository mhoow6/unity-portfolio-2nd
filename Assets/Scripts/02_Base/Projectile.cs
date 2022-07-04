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
    #endregion

    #region ������Ʈ Ǯ��
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

    #region �浹 ����
    protected void OnTriggerEnter(Collider other)
    {
        // ���� �浹�� Ǯ���� �ڱ� ��ȯ
        if (other.gameObject.layer == GameManager.GameDevelopSettings.TerrainLayermask)
        {
            StageManager.Instance.PoolSystem.Release(this);
            return;
        }

        // ĳ���Ϳ� �浹��
        var rhs = other.GetComponent<Character>();
        if (rhs != null && !other.CompareTag(m_Owner.tag))
        {
            var result = m_Owner.CalcuateDamage(rhs, m_DamageScale);

            // ���� ������
            rhs.Damaged(m_Owner, result.Item1, result.Item2);

            OnCollide(other);

            // Ǯ���� �ڱ� ��ȯ
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

    #region ����ü �߻�
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
        StageManager.Instance.PoolSystem.Release(this);
    }

    IEnumerator ShootParabolaCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        yield return null;
    }
    #endregion

    #region ������Ʈ Ǯ��
    bool m_Poolable;
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
