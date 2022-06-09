using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System;

public class CustomSlider : Display
{
    [Header("# ���� ����")]
    [SerializeField] int m_ElementCount;
    [SerializeField] Color m_BackgroundColor;
    [SerializeField] Color m_DeltaColor;
    [SerializeField] Color[] m_SliderColors;

    [Header("# ������ �ɼ�")]
    [SerializeField, ReadOnly] int m_SliderCount;
    [SerializeField, ReadOnly] int m_CurrentColorIndex;
    [SerializeField] CustomSliderElement m_ElementPrefab;
    [SerializeField, ReadOnly] float m_Value;

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
            float clamped = Mathf.Clamp(value, m_MinValue, m_MaxValue); 
            CustomSliderElement last = null;
            CustomSliderElement deltaLast = null;

            float expectedValue = 0f;
            float delta = m_Value - clamped;

            // �������� ��ȭ�� element ã��
            if (delta != 0)
            {
                (last, deltaLast) = TryGetValuedElements(delta);
                if (last != null && deltaLast != null)
                {
                    expectedValue = last.Value - delta;

                    // value�� �����ϴ� ���
                    if (expectedValue > 0)
                    {
                        while (expectedValue > last.MaxValue)
                        {
                            // ���� ��
                            last.Value = last.MaxValue;

                            // �������� ��ȭ�� element ã��
                            (last, deltaLast) = TryGetValuedElements(delta);

                            expectedValue -= last.MaxValue;
                        }
                        last.Value = expectedValue;
                        deltaLast.Value = expectedValue;
                        //ShowDelta(deltaLast, expectedValue, DeltaType.Slow);
                    }
                    // value�� �����ϴ� ���
                    else
                    {
                        while (expectedValue < 0)
                        {
                            // ������ element�� 0���� �����.
                            float lastValue = last.Value;
                            last.Value = 0;

                            // ��Ÿ�� ǥ��
                            deltaLast.Value = 0;
                            //ShowDelta(deltaLast, 0, DeltaType.Slow);

                            // ���� ��
                            delta = delta - lastValue;

                            // �������� ��ȭ�� element ã��
                            (last, deltaLast) = TryGetValuedElements(delta);

                            // �� �̻� ��ȭ�� element�� �� ã�� ��� ����
                            if (last == null && deltaLast == null)
                                expectedValue = 0;
                            else
                                expectedValue = last.Value - delta;
                        }
                        last.Value = expectedValue;
                        deltaLast.Value = expectedValue;
                        //ShowDelta(deltaLast, expectedValue, DeltaType.Slow);
                    }
                }
            }

            m_Value = clamped;
            m_OnValueUpdate?.Invoke(clamped);
        }
    }
    Action<float> m_OnValueUpdate;

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
    
    public void SetData(float minValue, float maxValue, float currentValue, Action<float> onValueUpdate = null)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Value = Mathf.Clamp(currentValue, m_MinValue, m_MaxValue);
        m_OnValueUpdate = onValueUpdate;

        float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
        float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;

        // �����̴� element ����
        int colorIndex = m_SliderCount - 1;
        for (int i = 0; i < m_SliderCount; i++)
        {
            float minRange = (m_MaxValue / m_SliderCount) * i;
            float maxRange = (m_MaxValue / m_SliderCount) * (i+1);

            if (currentValue >= minRange && currentValue < maxRange)
            {
                colorIndex = i;
                break;
            }
        }
        m_CurrentColorIndex = colorIndex;

        int backgroundIndex = colorIndex - 1;
        for (int i = 0; i < m_ElementCount; i++)
        {
            // ��׶��� ����
            var back = m_BackElements[i];
            if (backgroundIndex >= 0)
                back.Image.color = m_SliderColors[backgroundIndex];
            else
                back.Image.color = m_BackgroundColor;

            // ����Ʈ ����
            var front = m_FrontElements[i];
            front.Image.color = m_SliderColors[m_CurrentColorIndex];
        }

        // �� ���� �����̴��� element�� ������ value�� ��
        // ������� �� �ս��� �����Ѽ� ����� ǥ��
        float elementValueSum = (elementMaxValue * m_ElementCount) - (m_MaxValue - currentValue);

        // �����̴� element ��
        for (int i = 0; i < m_ElementCount; i++)
        {
            var delta = m_DeltaElements[i];
            var front = m_FrontElements[i];
            float elementValue = 0f;

            if (elementValueSum - elementMaxValue >= 0)
            {
                elementValue = elementMaxValue;
                elementValueSum -= elementMaxValue;
            }
            else
            {
                elementValue = elementValueSum;
                elementValueSum = 0;
            }

            delta.SetData(elementMinValue, elementMaxValue, elementValue);
            front.SetData(elementMinValue, elementMaxValue, elementValue);
        }

        
    }

    public void CreateElements()
    {
        if (m_BackElements.Count > 0)
        {
            Debug.LogError($"������ element�� �����ϰ� ���ּ���.");
            return;
        }

        if (Application.isPlaying)
        {
            Debug.LogError($"���� �����߿� �����̴��� ����� �����ų �� �����ϴ�. ������ ��忡�� �������ּ���.");
            return;
        }

        m_Flexible = false;

        // ��׶���
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_BackgroundLayout.transform);
            inst.name = $"Background Element ({i})";

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
            inst.name = $"Delta Element ({i})";

            inst.Image.color = m_DeltaColor;
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
            m_DeltaElements.Add(inst);
        }

        // ���� �����̴�
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_FrontLayout.transform);
            inst.name = $"Front Element ({i})";

            inst.Image.color = m_SliderColors[m_SliderColors.Length - 1];
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
            m_FrontElements.Add(inst);
        }
        m_SliderCount = m_SliderColors.Length;
        m_CurrentColorIndex = m_SliderColors.Length - 1;

        Debug.LogWarning($"���� ���� ���� {m_DeltaLayout}�� {m_FrontLayout}�� Horizontal Layout Group�� �ݵ�� ���ּ���!");
        EditorUtility.SetDirty(this);
    }

    public void DestroyElements()
    {
        if (Application.isPlaying)
        {
            Debug.LogError($"���� �����߿� �����̴��� ����� �����ų �� �����ϴ�. ������ ��忡�� �������ּ���.");
            return;
        }

        // ��׶���
        for (int i = 0; i < m_BackElements.Count; i++)
        {
            var child = m_BackElements[i];
            DestroyImmediate(child.gameObject);
        }

        // ��ȭ��
        for (int i = 0; i < m_DeltaElements.Count; i++)
        {
            var child = m_DeltaElements[i];
            DestroyImmediate(child.gameObject);
        }

        // ���� �����̴�
        for (int i = 0; i < m_FrontElements.Count; i++)
        {
            var child = m_FrontElements[i];
            DestroyImmediate(child.gameObject);
        }

        m_BackElements.Clear();
        m_DeltaElements.Clear();
        m_FrontElements.Clear();

        m_SliderCount = 0;
        m_CurrentColorIndex = 0;

        EditorUtility.SetDirty(this);
    }

    // UNDONE
    void ShowDelta(CustomSliderElement deltaElement, float desiredValue, DeltaType type) { }

    /// <summary>
    /// Value ������Ƽ���� �ʿ��� �Լ�. value�� �����ų ������ element�� ���մϴ�.
    /// </summary>
    /// <param name="diff">���� �����̴��� value��, �����̴��� ����� value���� ���̰�</param>
    /// <returns></returns>
    (CustomSliderElement front, CustomSliderElement delta) TryGetValuedElements(float diff)
    {
        (CustomSliderElement, CustomSliderElement) result;
        result.Item1 = null;
        result.Item2 = null;

        // �������� ��ȭ�� element ã��
        try
        {
            if (diff > 0)
            {
                result.Item1 = m_FrontElements.Last((element) => element.Value > 0);
                result.Item2 = m_DeltaElements.Last((element) => element.Value > 0);
            }
            else
            {
                result.Item1 = m_FrontElements.Last((element) => element.Value > 0 && element.Value < element.MaxValue);
                result.Item2 = m_DeltaElements.Last((element) => element.Value > 0 && element.Value < element.MaxValue);
            }
        }
        // �� �̻� ���� ������ element�� ���� ���
        catch (Exception)
        {
            // ���� �������ε�, ���� element���� ���� 0�̿��� ����� ����
            var zeros = m_FrontElements.FindAll(element => element.Value == 0);
            var deltaZeros = m_DeltaElements.FindAll(element => element.Value == 0);
            if (diff < 0 && zeros.Count > 0)
            {
                // �׳� �ڿ��� �̿��϶�� �ؾ��Ѵ�.
                result.Item1 = zeros[0];
                result.Item2 = deltaZeros[0];
            }
            else
            {
                int nextSliderIndex = diff > 0 ? --m_CurrentColorIndex : ++m_CurrentColorIndex;

                if (nextSliderIndex >= 0)
                {

                    for (int i = 0; i < m_ElementCount; i++)
                    {
                        var cur = m_FrontElements[i];
                        var curdelta = m_DeltaElements[i];
                        var back = m_BackElements[i];

                        // ��׶��� ���� �ʱ�ȭ
                        int backgroundIndex = nextSliderIndex - 1;
                        if (backgroundIndex >= 0)
                            back.Image.color = m_SliderColors[backgroundIndex];
                        else
                            back.Image.color = m_BackgroundColor;

                        // element ���� �ʱ�ȭ
                        float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
                        float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;
                        cur.Image.color = m_SliderColors[nextSliderIndex];

                        // element �� ���� ����
                        if (diff > 0)
                        {
                            cur.SetData(elementMinValue, elementMaxValue, elementMaxValue);
                            curdelta.SetData(elementMinValue, elementMaxValue, elementMaxValue);
                        }
                        else
                        {
                            cur.SetData(elementMinValue, elementMaxValue, 0);
                            curdelta.SetData(elementMinValue, elementMaxValue, 0);
                        }
                    }

                    // �ٽ� ã�ƺ���
                    if (diff > 0)
                    {
                        result.Item1 = m_FrontElements.Last();
                        result.Item2 = m_DeltaElements.Last();
                    }
                    else
                    {
                        result.Item1 = m_FrontElements.First();
                        result.Item2 = m_DeltaElements.First();
                    }
                }

            }
        }

        return result;
    }
}

enum DeltaType
{
    Fast,
    Slow
}
