using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HonkaiSliderFrame : Display
{
    [SerializeField] Image m_Image;
    [SerializeField,ReadOnly] float m_MaxGauge;
    [ReadOnly] public float Gauge;

    public void SetData(Color color, float maxGauge)
    {
        m_Image.color = color;
        m_MaxGauge = maxGauge;
        Gauge = maxGauge;
    }
}
