using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
    public void Shoot(Vector3 direction, float moveSpeed, int lifeTime)
    {
        StartCoroutine(ShootCoroutine(direction, moveSpeed, lifeTime));
    }

    IEnumerator ShootCoroutine(Vector3 direction, float moveSpeed, int lifeTime)
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.tag}");
    }
}
