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
    public void ShootStraight(Vector3 direction, float moveSpeed, int lifeTime)
    {
        StartCoroutine(ShootStraightCoroutine(direction, moveSpeed, lifeTime));
    }

    public void ShootParabola(Vector3 startPosition, Vector3 endPosition, float height, float lifeTime)
    {
        StartCoroutine(ShootParabolaCoroutine(startPosition, endPosition, height, lifeTime));
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
            rhs.Damaged(m_Owner, result.Damage, result.IsCrit);

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
    Parabola
}
