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

    float m_Value;
    float m_MinValue; // 100
    float m_MaxValue; // 300
    public float OriginWidth { get; private set; }

    public void SetData(float minValue, float maxValue, float value)
    {
        StartCoroutine(SetDataCoroutine(minValue, maxValue, value));
    }

    public float SimulateWidth(float sliderValue)
    {
        float ratio = (sliderValue - m_MinValue) / m_MaxValue;
        return Width * ratio;
    }

    IEnumerator SetDataCoroutine(float minValue, float maxValue, float value)
    {
        // Horizontal Layout Group으로 인해 sizeDelta는 늦게 한 프레임 결정된다.
        yield return null;
        OriginWidth = RectTransform.sizeDelta.x;

        m_MinValue = minValue;
        Value = value; // UNDONE: 왜 NaN?
        m_MaxValue = maxValue;
    }
}
