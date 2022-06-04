using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSliderElement : Display
{
    public Image Image;
    public RectTransform RectTransform;
    public float Width => RectTransform.sizeDelta.x;
    public float Value
    {
        get
        {
            return m_Value;
        }
        set
        {
            // 이미지 크기 줄이기
            float clamped = value - m_MinValue;

            float ratio = clamped / m_MaxValue;
            RectTransform.sizeDelta = new Vector2(OriginWidth * ratio, RectTransform.sizeDelta.y);

            m_Value = clamped;
        }
    }

    [SerializeField] Animator m_Animator;
    float m_Value;
    float m_MinValue; // 100
    float m_MaxValue; // 300
    float m_PrevDesired;
    bool m_SmoothValueCoroutineStart;

    public float OriginWidth { get; private set; }
    bool m_OriginWidthSet;

    const float VALUE_SPEED = 0.2f;

    public void SetData(float minValue, float maxValue)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Value = maxValue;

        if (!m_OriginWidthSet)
        {
            OriginWidth = RectTransform.sizeDelta.x;
            m_OriginWidthSet = true;
        }

        float ratio = m_Value / m_MaxValue;
        RectTransform.sizeDelta = new Vector2(OriginWidth * ratio, RectTransform.sizeDelta.y);
    }

    public void SmoothValue(float desired)
    {
        // 이미 SmoothValue 하고 있으면 그것을 중지시켜야 한다.
        if (m_SmoothValueCoroutineStart)
        {
            Value = m_PrevDesired;
            if (m_Animator != null)
                m_Animator.SetBool("OnDelta", false);

            StopAllCoroutines();
        }

        StartCoroutine(SmoothValueCoroutine(desired));
    }

    IEnumerator SmoothValueCoroutine(float desired)
    {
        float clamped = desired - m_MinValue;
        float desiredRatio = clamped / m_MaxValue;
        float prevRatio = m_Value / m_MaxValue;
        float controlRatio = prevRatio;

        m_SmoothValueCoroutineStart = true;
        m_PrevDesired = desired;
        m_Value = desired;

        if (m_Animator != null)
            m_Animator.SetBool("OnDelta", true);

        // 0.4f < 1.0f
        if (desiredRatio <= prevRatio)
        {
            while (desiredRatio < controlRatio)
            {
                controlRatio -= VALUE_SPEED * Time.deltaTime;

                RectTransform.sizeDelta = new Vector2(OriginWidth * controlRatio, RectTransform.sizeDelta.y);

                yield return null;
            }
        }
        else
        {
            while (desiredRatio > controlRatio)
            {
                controlRatio += VALUE_SPEED * Time.deltaTime;

                RectTransform.sizeDelta = new Vector2(OriginWidth * controlRatio, RectTransform.sizeDelta.y);

                yield return null;
            }
        }

        if (m_Animator != null)
            m_Animator.SetBool("OnDelta", false);

        m_SmoothValueCoroutineStart = false;
        Debug.Log($"SmoothValue 종료");
    }
}
