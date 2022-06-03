using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System;

public class CustomSlider : Display
{
    [SerializeField] int m_ElementCount;
    
    [SerializeField] Color m_BackgroundColor;
    [SerializeField] Color m_DeltaColor;
    [SerializeField] Color[] m_SliderColors;
    [SerializeField] HorizontalLayoutGroup m_BackgroundLayout;

    [SerializeField] HorizontalLayoutGroup m_DeltaLayout;
    [SerializeField, ReadOnly] List<CustomSliderElement> m_DeltaElements = new List<CustomSliderElement>();

    [SerializeField] HorizontalLayoutGroup m_FrontLayout;
    [SerializeField, ReadOnly] List<CustomSliderElement> m_FrontElements = new List<CustomSliderElement>();

    [SerializeField] CustomSliderElement m_ElementPrefab;

    public float Value
    {
        get
        {
            return m_Value;
        }
        set
        {
            if (value >= m_MinValue)
            {
                m_Flexible = true;

                float delta = m_Value - value - m_MinValue;
                var last = m_FrontElements.Last((element) => element.Width > 0);

                float currentWidth = last.Width;
                float expectedWidth = last.SimulateWidth(delta);

                // ������ element�� value�� �ʰ��ϴ� ���� �޾ƹ��� ���
                while (expectedWidth > currentWidth)
                {
                    // ������ element�� 0���� �����.
                    float lastValue = last.Value;
                    last.Value = 0;
                    // ���� ��
                    delta = delta - lastValue;

                    // ���ο� �������� ���Ѵ�.
                    last = m_FrontElements.Last((element) => element.Width > 0);

                    // ���� �������� ã�� ���ߴٸ�.. �Ѿ�� ��
                    if (last == null)
                    {
                        int nextSliderIndex = --m_CurrentColorIndex;

                        // �� �̻��� �����̴��� ���ٸ� ��
                        if (nextSliderIndex + 1 > m_SliderCount)
                            break;

                        // element �� ���� ����
                        for (int i = 0; i < m_ElementCount; i++)
                        {
                            var cur = m_FrontElements[i];
                            cur.Image.color = m_SliderColors[nextSliderIndex];
                            cur.RectTransform.pivot = new Vector2(0, 0.5f);

                            float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
                            float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;
                            cur.SetData(elementMinValue, elementMaxValue, elementMaxValue);
                        }

                        // �������� �� ã�ƺ���.
                        last = m_FrontElements.Last((element) => element.Width > 0);
                    }

                    // �������� ������ width�� ��� ������ ����.
                    if (last != null)
                    {
                        currentWidth = last.Width;
                        expectedWidth = last.SimulateWidth(delta);
                    }

                }

                if (last != null)
                {
                    float rest = last.Value - delta;
                    last.Value = rest;
                }


                m_Value = value;
            }
            else
                m_Value = m_MinValue;
        }
    }

    bool m_Flexible
    {
        get
        {
            return !m_DeltaLayout.enabled & !m_FrontLayout.enabled;
        }
        set
        {
            m_DeltaLayout.enabled = !value;
            m_FrontLayout.enabled = !value;
        }
    }
    float m_Value;
    float m_MinValue;
    float m_MaxValue;
    int m_CurrentColorIndex;
    int m_SliderCount;

    private void Start()
    {
        SetData(0, 1000);
        m_SliderCount = m_SliderColors.Length;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Value = 500;
    }

    public void SetData(float minValue, float maxValue)
    {
        m_MinValue = minValue;
        m_Value = maxValue;
        m_MaxValue = maxValue;

        float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
        float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;
        foreach (var delta in m_DeltaElements)
            delta.SetData(elementMinValue, elementMaxValue, elementMaxValue);

        foreach (var delta in m_FrontElements)
            delta.SetData(elementMinValue, elementMaxValue, elementMaxValue);
    }

    public void CreateElements()
    {
        // ��׶���
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_BackgroundLayout.transform);
            inst.Image.color = m_BackgroundColor;
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
        }

        // ��ȭ��
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_DeltaLayout.transform);
            inst.Image.color = m_DeltaColor;
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
            m_DeltaElements.Add(inst);
        }

        // ���� �����̴�
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_FrontLayout.transform);
            inst.Image.color = m_SliderColors[m_SliderColors.Length - 1];
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
            m_FrontElements.Add(inst);
        }
        m_CurrentColorIndex = m_SliderColors.Length - 1;

        EditorUtility.SetDirty(this);
    }

    public void DestroyElements()
    {
        m_DeltaElements.Clear();
        m_FrontElements.Clear();
        m_CurrentColorIndex = 0;

        // ��׶���
        for (int i = 0; i < m_BackgroundLayout.transform.childCount; i++)
        {
            var child = m_BackgroundLayout.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        // ��ȭ��
        for (int i = 0; i < m_DeltaLayout.transform.childCount; i++)
        {
            var child = m_DeltaLayout.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        // ���� �����̴�
        for (int i = 0; i < m_FrontLayout.transform.childCount; i++)
        {
            var child = m_FrontLayout.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        EditorUtility.SetDirty(this);
    }
}
