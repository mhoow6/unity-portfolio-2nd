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

        m_InstantiateEffects.ForEach(p => StageManager.PoolSystem.Release(p));
        m_InstantiateEffects.Clear();
    }

    IEnumerator CollideEffectCoroutine()
    {
        var gm = GameManager.Instance;
        var sm = StageManager.Instance;

        while (true)
        {
            // 움직일 때 새로 이펙트 생성
            if (gm.Player.MoveVector.magnitude != 0 && m_InstantiateEffects.Count < MAXIMUM_INSTANTIATE_EFFECT)
            {
                var particle = StageManager.PoolSystem.Load<Effect>($"{gm.Config.EffectResourcePath}/FX_Direction_Arrows_03");
                Vector3 characterPosition = gm.Player.CurrentCharacter.transform.position;
                m_InstantiateEffects.Add(particle);
                particle.transform.SetPositionAndRotation(characterPosition, Quaternion.Euler(m_BlockEffectSpawnRotation));
            }
            // 가만히 있을때에 아예 없애주자
            else
            {
                m_InstantiateEffects.ForEach(p => StageManager.PoolSystem.Release(p));
                m_InstantiateEffects.Clear();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
