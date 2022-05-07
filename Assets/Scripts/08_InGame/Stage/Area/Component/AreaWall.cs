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
        // �� �浹�� ����Ʈ �߻�
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
            // ������ �� ���� ����Ʈ ����
            if (player.MoveVector.magnitude != 0 && m_InstantiateEffects.Count < m_MAXIMUM_INSTANTIATE_EFFECT)
            {
                var particle = pool.Load<Effect>($"{path}/FX_Direction_Arrows_03");
                Vector3 characterPosition = charcter.transform.position;
                m_InstantiateEffects.Add(particle);
                particle.transform.SetPositionAndRotation(characterPosition, Quaternion.Euler(m_BlockEffectSpawnRotation));
            }
            // ������ �������� �ƿ� ��������
            else
            {
                m_InstantiateEffects.ForEach(p => StageManager.Instance.Pool.Release(p));
                m_InstantiateEffects.Clear();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
