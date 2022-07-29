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

        m_InstantiateEffects.ForEach(p => StageManager.Instance.PoolSystem.Release(p));
        m_InstantiateEffects.Clear();
    }

    IEnumerator CollideEffectCoroutine()
    {
        var sm = StageManager.Instance;

        while (true)
        {
            // ������ �� ���� ����Ʈ ����
            if (sm.Player.MoveVector.magnitude != 0 && m_InstantiateEffects.Count < MAXIMUM_INSTANTIATE_EFFECT)
            {
                var particle = sm.PoolSystem.Load<Effect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/Common_AreaWallBlock");
                Vector3 characterPosition = sm.Player.CurrentCharacter.transform.position;
                Vector3 effectSpawnPosition = characterPosition + new Vector3(0, 0.3f, 0);

                m_InstantiateEffects.Add(particle);
                particle.transform.SetPositionAndRotation(effectSpawnPosition, Quaternion.Euler(m_BlockEffectSpawnRotation));
            }
            // ������ �������� �ƿ� ��������
            else
            {
                m_InstantiateEffects.ForEach(p => sm.PoolSystem.Release(p));
                m_InstantiateEffects.Clear();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
