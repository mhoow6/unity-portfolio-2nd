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
        // �� �浹�� ����Ʈ �߻�
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
            // ������ �� ���� ����Ʈ ����
            if (gm.Player.MoveVector.magnitude != 0 && m_InstantiateEffects.Count < MAXIMUM_INSTANTIATE_EFFECT)
            {
                var particle = StageManager.PoolSystem.Load<Effect>($"{gm.Config.EffectResourcePath}/FX_Direction_Arrows_03");
                Vector3 characterPosition = gm.Player.CurrentCharacter.transform.position;
                m_InstantiateEffects.Add(particle);
                particle.transform.SetPositionAndRotation(characterPosition, Quaternion.Euler(m_BlockEffectSpawnRotation));
            }
            // ������ �������� �ƿ� ��������
            else
            {
                m_InstantiateEffects.ForEach(p => StageManager.PoolSystem.Release(p));
                m_InstantiateEffects.Clear();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
