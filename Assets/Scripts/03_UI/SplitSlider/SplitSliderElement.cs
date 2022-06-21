using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplitSliderElement : Display
{
    public Image Image;
    public RectTransform RectTransform;
    public float Value
    {
        get
        {
            return m_Value;
        }
        set
        {
            // 이미지 크기 줄이기
            float clamped = Mathf.Clamp(value, m_MinValue, m_MaxValue);

            float ratio = clamped / m_MaxValue;
            RectTransform.sizeDelta = new Vector2(m_OriginWidth * ratio, RectTransform.sizeDelta.y);

            m_Value = clamped;
        }
    }

    [SerializeField] Animator m_Animator;
    [SerializeField, ReadOnly] float m_Value;

    public float MinValue => m_MinValue;
    float m_MinValue;
    public float MaxValue => m_MaxValue;
    float m_MaxValue;

    float m_OriginWidth;

    bool m_OriginWidthSet;

    const float VALUE_SPEED = 0.2f;

    public void SetData(float minValue, float maxValue, float value)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Value = Mathf.Clamp(value, m_MinValue, m_MaxValue);

        if (!m_OriginWidthSet)
        {
            m_OriginWidth = RectTransform.sizeDelta.x;
            m_OriginWidthSet = true;
        }

        float ratio = m_Value / m_MaxValue;
        RectTransform.sizeDelta = new Vector2(m_OriginWidth * ratio, RectTransform.sizeDelta.y);
    }

}
