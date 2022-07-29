using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaWall : AreaComponent
{
    [SerializeField] Vector3 m_BlockEffectSpawnRotation;

    List<Effect> m_InstantiateEffects = new List<Effect>();
    const int MAXIMUM_INSTANTIATE_EFFECT = 3;

    private void OnCollisionEnter(Collision collision)
    {
        // 벽 충돌시 이펙트 발생
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(CollideEffectCoroutine());
    }

    private void OnCollisionExit(Collision collision)
    {
        StopAllCoroutines();

        m_InstantiateEffects.ForEach(p => StageManager.Instance.PoolSystem.Release(p));
        m_InstantiateEffects.Clear();
    }

    IEnumerator CollideEffectCoroutine()
    {
        var sm = StageManager.Instance;

        while (true)
        {
            // 움직일 때 새로 이펙트 생성
            if (sm.Player.MoveVector.magnitude != 0 && m_InstantiateEffects.Count < MAXIMUM_INSTANTIATE_EFFECT)
            {
                var particle = sm.PoolSystem.Load<Effect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/Common_AreaWallBlock");
                Vector3 characterPosition = sm.Player.CurrentCharacter.transform.position;
                Vector3 effectSpawnPosition = characterPosition + new Vector3(0, 0.3f, 0);

                m_InstantiateEffects.Add(particle);
                particle.transform.SetPositionAndRotation(effectSpawnPosition, Quaternion.Euler(m_BlockEffectSpawnRotation));
            }
            // 가만히 있을때에 아예 없애주자
            else
            {
                m_InstantiateEffects.ForEach(p => sm.PoolSystem.Release(p));
                m_InstantiateEffects.Clear();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
