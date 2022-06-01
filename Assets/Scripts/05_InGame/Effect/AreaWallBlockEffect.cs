using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWallBlockEffect : Effect
{
    public override ObjectCode Code => ObjectCode.EFFECT_CharacterHitWall;
    const float INVISIBLE_DURATION = 2f;

    protected override void OnPoolReleased()
    {
        // 서서히 사라지게 한다.
        StartCoroutine(AlphaBlendingCoroutine());
    }

    IEnumerator AlphaBlendingCoroutine()
    {
        float timer = 0f;
        var originColor = EffectColor;

        // 2초동안 알파블랜딩
        while (timer < INVISIBLE_DURATION)
        {
            // 뭔가 하고 있으니 풀링은 당연히 불가능
            Poolable = false;

            timer += Time.deltaTime;
            var currentColor = EffectColor;

            Color desired = Color.Lerp(EffectColor, new Color(currentColor.r, currentColor.g, currentColor.b, 0), Time.deltaTime);
            EffectColor = desired;
            yield return null;
        }
        m_ParticleSystem.Stop(true);
        EffectColor = originColor;
        Poolable = true;
        gameObject.SetActive(false);
    }
}
