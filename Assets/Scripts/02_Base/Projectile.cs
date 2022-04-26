using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
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
        transform.forward = direction.normalized;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            yield return null;
        }
        // TODO: 오브젝트 풀에 관리되게끔 해야함.
        Destroy(gameObject);
    }

    IEnumerator ShootParabolaCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
    {
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.tag}");
    }

    public enum TrajectoryType
    {
        /// <summary> 직선 </summary>
        Straight,
        /// <summary> 포물선 </summary>
        Parabola
    }
}
