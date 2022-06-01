using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWallBlockEffect : Effect
{
    public override ObjectCode Code => ObjectCode.EFFECT_CharacterHitWall;
    const float INVISIBLE_DURATION = 2f;

    protected override void OnPoolReleased()
    {
        // ������ ������� �Ѵ�.
        StartCoroutine(AlphaBlendingCoroutine());
    }

    IEnumerator AlphaBlendingCoroutine()
    {
        float timer = 0f;
        var originColor = EffectColor;

        // 2�ʵ��� ���ĺ���
        while (timer < INVISIBLE_DURATION)
        {
            // ���� �ϰ� ������ Ǯ���� �翬�� �Ұ���
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
