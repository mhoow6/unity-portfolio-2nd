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
    [SerializeField, ReadOnly] float m_Value;
    [SerializeField] CustomSliderElement m_ElementPrefab;

    [Space(10)]
    [SerializeField] Color m_BackgroundColor;
    [SerializeField] Color m_DeltaColor;
    [SerializeField] Color[] m_SliderColors;

    [Space(10)]
    [SerializeField] HorizontalLayoutGroup m_BackgroundLayout;
    [SerializeField, ReadOnly] List<CustomSliderElement> m_BackElements = new List<CustomSliderElement>();

    [SerializeField] HorizontalLayoutGroup m_DeltaLayout;
    [SerializeField, ReadOnly] List<CustomSliderElement> m_DeltaElements = new List<CustomSliderElement>();

    [SerializeField] HorizontalLayoutGroup m_FrontLayout;
    [SerializeField, ReadOnly] List<CustomSliderElement> m_FrontElements = new List<CustomSliderElement>();

    

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
                float delta = m_Value - value - m_MinValue;
                var last = m_FrontElements.Last((element) => element.Value > 0);
                // var deltaLast = m_DeltaElements.Last((element) => element.Value > 0);

                float currentValue = last.Value;
                float expectedValue = last.Value - delta;

                // ������ element�� value�� �ʰ��ϴ� ���� �޾ƹ��� ���
                while (expectedValue < 0)
                {
                    // ������ element�� 0���� �����.
                    float lastValue = last.Value;
                    last.Value = 0;

                    // ��Ÿ ��ȭ
                    // deltaLast.SmoothValue(0);

                    // ���� ��
                    delta = delta - lastValue;

                    // ���ο� �������� ���Ѵ�.
                    try
                    {
                        last = m_FrontElements.Last((element) => element.Value > 0);
                        // deltaLast = m_DeltaElements.Last((element) => element.Value > 0);
                    }
                    catch (Exception e)
                    {
                        int nextSliderIndex = --m_CurrentColorIndex;

                        // ������ �����̴��� ��� ��׶��� �� ����
                        if (nextSliderIndex == 0)
                        {
                            // ��׶���
                            for (int i = 0; i < m_ElementCount; i++)
                            {
                                var back = m_BackElements[i];
                                back.Image.color = m_BackgroundColor;
                            }
                        }

                        // �� �̻��� �����̴��� ���ٸ� ��
                        if (nextSliderIndex < 0)
                            break;

                        // element �� ���� ����
                        for (int i = 0; i < m_ElementCount; i++)
                        {
                            var cur = m_FrontElements[i];
                            var curdelta = m_DeltaElements[i];

                            cur.Image.color = m_SliderColors[nextSliderIndex];
                            cur.RectTransform.pivot = new Vector2(0, 0.5f);
                            // curdelta.Image.color = m_DeltaColor;
                            // curdelta.RectTransform.pivot = new Vector2(0, 0.5f);

                            float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
                            float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;

                            cur.SetData(elementMinValue, elementMaxValue);
                            // curdelta.SetData(elementMinValue, elementMaxValue);
                        }
                        last = m_FrontElements.Last((element) => element.Value > 0);
                    }
                    finally
                    {
                        // �ٽ� �� �纻��.
                        currentValue = last.Value;
                        expectedValue = last.Value - delta;
                    }
                }


                if (last != null)
                {
                    last.Value = expectedValue;

                    // ��Ÿ ��ȭ
                    // deltaLast.SmoothValue(expectedValue);
                }

                m_Value = value;
            }
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
    
    float m_MinValue;
    float m_MaxValue;
    int m_CurrentColorIndex;
    int m_SliderCount;

    private void Start()
    {
        m_SliderCount = m_SliderColors.Length;
        m_CurrentColorIndex = m_SliderColors.Length - 1;

        SetData(0, 1000);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Value -= 100;
    }

    public void SetData(float minValue, float maxValue)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Value = maxValue;

        float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
        float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;
        foreach (var delta in m_DeltaElements)
            delta.SetData(elementMinValue, elementMaxValue);

        foreach (var delta in m_FrontElements)
            delta.SetData(elementMinValue, elementMaxValue);
    }

    public void CreateElements()
    {
        m_Flexible = false;

        // ��׶���
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_BackgroundLayout.transform);

            if (m_SliderColors.Length >= 2)
                inst.Image.color = m_SliderColors[m_SliderColors.Length - 2];
            else
                inst.Image.color = m_BackgroundColor;

            inst.RectTransform.pivot = new Vector2(0, 0.5f);
            m_BackElements.Add(inst);
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
        

        EditorUtility.SetDirty(this);
    }

    public void DestroyElements()
    {
        m_BackElements.Clear();
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
