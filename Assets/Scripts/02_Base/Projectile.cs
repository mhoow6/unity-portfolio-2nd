using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),(typeof(Rigidbody)))]
public class Projectile : BaseObject
{
    [SerializeField] Rigidbody m_RigidBody;
    [SerializeField] SphereCollider m_SphereCollider;

    const float m_SHOOT_VELOCITY = 2f;

    protected void OnTriggerEnter(Collider other)
    {
        OnCollide(other);
    }

    protected virtual void OnCollide(Collider other) { }

    #region ����ü �߻�
    public void Shoot(Vector3 direction, TrajectoryType trajectoryType, float moveSpeed, int lifeTime)
    {
        switch (trajectoryType)
        {
            case TrajectoryType.Straight:
                StartCoroutine(ShootStraightCoroutine(direction, moveSpeed, lifeTime));
                break;
            case TrajectoryType.Parabola:
                StartCoroutine(ShootParabolaCoroutine(direction, moveSpeed, lifeTime));
                break;
            default:
                break;
        }
    }

    IEnumerator ShootStraightCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        float timer = 0f;
        m_RigidBody.isKinematic = true;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;

            yield return null;
        }
        m_RigidBody.isKinematic = false;
        // TODO: ������Ʈ Ǯ�� �����ǰԲ� �ؾ���.
        Destroy(gameObject);
    }

    IEnumerator ShootParabolaCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        float timer = 0f;
        float spawnZRotation = transform.eulerAngles.z;

        m_RigidBody.isKinematic = false;
        // direction �������� moveSpeed��ŭ �ӷ��� �ش�.
        m_RigidBody.velocity = direction.normalized * moveSpeed * m_SHOOT_VELOCITY;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            float angle = Mathf.Atan2(m_RigidBody.velocity.y, m_RigidBody.velocity.z);
            float addAngle = transform.eulerAngles.z + angle;
            var z = addAngle > spawnZRotation ? spawnZRotation : addAngle;

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, z);
            yield return new WaitForFixedUpdate();
        }
        // TODO: ������Ʈ Ǯ�� �����ǰԲ� �ؾ���.
        Destroy(gameObject);
    }
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
