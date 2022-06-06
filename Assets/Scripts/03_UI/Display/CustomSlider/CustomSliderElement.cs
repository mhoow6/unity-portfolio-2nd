using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSliderElement : Display
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
            float clamped = value - m_MinValue;

            float ratio = clamped / m_MaxValue;
            RectTransform.sizeDelta = new Vector2(m_OriginWidth * ratio, RectTransform.sizeDelta.y);

            m_Value = clamped;
        }
    }

    [SerializeField] Animator m_Animator;
    [SerializeField, ReadOnly] float m_Value;

    float m_MinValue;
    float m_MaxValue;
    float m_OriginWidth;
    bool m_OriginWidthSet;

    const float VALUE_SPEED = 0.2f;

    public void SetData(float minValue, float maxValue, float value)
    {
        
    }
}
