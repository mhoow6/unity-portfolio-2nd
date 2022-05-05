using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWallBlockEffect : Effect
{
    const float m_INVISIBLE_DURATION = 2f;

    protected override void ReleaseAction()
    {
        // 서서히 사라지게 한다.
        StartCoroutine(AlphaBlendingCoroutine());
    }

    IEnumerator AlphaBlendingCoroutine()
    {
        float timer = 0f;
        var originColor = m_EffectColor;

        // 2초동안 알파블랜딩
        while (timer < m_INVISIBLE_DURATION)
        {
            // 뭔가 하고 있으니 풀링은 당연히 불가능
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
