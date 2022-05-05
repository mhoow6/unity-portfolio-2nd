using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWallBlockEffect : Effect
{
    const float m_INVISIBLE_DURATION = 2f;

    protected override void ReleaseAction()
    {
        // ������ ������� �Ѵ�.
        StartCoroutine(AlphaBlendingCoroutine());
    }

    IEnumerator AlphaBlendingCoroutine()
    {
        float timer = 0f;
        var originColor = m_EffectColor;

        // 2�ʵ��� ���ĺ���
        while (timer < m_INVISIBLE_DURATION)
        {
            // ���� �ϰ� ������ Ǯ���� �翬�� �Ұ���
            Poolable = false;

            timer += Time.deltaTime;
            var currentColor = m_EffectColor;

            Color desired = Color.Lerp(m_EffectColor, new Color(currentColor.r, currentColor.g, currentColor.b, 0), Time.deltaTime);
            m_EffectColor = desired;
            yield return null;
        }
        m_ParticleSystem.Stop(true);
        m_EffectColor = originColor;
        Poolable = true;
        gameObject.SetActive(false);
    }
}
