using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaWall : AreaComponent
{
    [SerializeField] Vector3 m_BlockEffectSpawnRotation;

    List<Effect> m_InstantiateEffects = new List<Effect>();
    const int m_MAXIMUM_INSTANTIATE_EFFECT = 3;

    private void OnTriggerEnter(Collider other)
    {
        // 벽 충돌시 이펙트 발생
        if (other.CompareTag("Player"))
        {
            var player = GameManager.Instance.Player;
            StartCoroutine(CollideEffectCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();

        m_InstantiateEffects.ForEach(p => StageManager.Instance.Pool.Release(p));
        m_InstantiateEffects.Clear();
    }

    IEnumerator CollideEffectCoroutine()
    {
        var path = GameManager.Instance.Config.EffectResourcePath;
        var pool = StageManager.Instance.Pool;
        var player = GameManager.Instance.Player;
        var charcter = GameManager.Instance.Player.CurrentCharacter;

        while (true)
        {
            // 움직일 때 새로 이펙트 생성
            if (player.MoveVector.magnitude != 0 && m_InstantiateEffects.Count < m_MAXIMUM_INSTANTIATE_EFFECT)
            {
                var particle = pool.Load<Effect>($"{path}/FX_Direction_Arrows_03");
                Vector3 characterPosition = charcter.transform.position;
                m_InstantiateEffects.Add(particle);
                particle.transform.SetPositionAndRotation(characterPosition, Quaternion.Euler(m_BlockEffectSpawnRotation));
            }
            // 가만히 있을때에 아예 없애주자
            else
            {
                m_InstantiateEffects.ForEach(p => StageManager.Instance.Pool.Release(p));
                m_InstantiateEffects.Clear();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
