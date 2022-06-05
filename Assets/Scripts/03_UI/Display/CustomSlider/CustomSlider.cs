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
                var deltaLast = m_DeltaElements.Last((element) => element.Value > 0);

                float expectedValue = last.Value - delta;

                // 마지막 element의 value를 초과하는 값을 받아버릴 경우
                while (expectedValue < 0)
                {
                    // 마지막 element를 0으로 만든다.
                    float lastValue = last.Value;
                    last.Value = 0;

                    // 누군가 이미 smoothValue를 하는 경우 마무리를 지어줘야 함
                    var smoothDeltaList = m_DeltaElements.FindAll((element) => element.SmoothValueTasking);
                    if (smoothDeltaList != null)
                    {
                        foreach (var smoothDelta in smoothDeltaList)
                            smoothDelta.StopSmoothValue();
                    }
                    

                    // 델타 변화
                    deltaLast.StartSmoothValue(0);

                    // 남은 양
                    delta = delta - lastValue;

                    // 새로운 마지막을 구한다.
                    try
                    {
                        last = m_FrontElements.Last((element) => element.Value > 0);
                        deltaLast = m_DeltaElements.Last((element) => element.Value > 0);
                    }
                    // 못 구했다면..
                    catch (Exception e)
                    {
                        int nextSliderIndex = --m_CurrentColorIndex;

                        // 더 이상의 슬라이더는 밑에 있는 과정은 의미가 없다
                        if (nextSliderIndex < 0)
                            break;

                        // element 값 새로 세팅
                        for (int i = 0; i < m_ElementCount; i++)
                        {
                            var cur = m_FrontElements[i];
                            var curdelta = m_DeltaElements[i];
                            var back = m_BackElements[i];

                            int backgroundIndex = nextSliderIndex - 1;
                            if (backgroundIndex >= 0)
                                back.Image.color = m_SliderColors[backgroundIndex];
                            else
                                back.Image.color = m_BackgroundColor;

                            float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
                            float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;
                            cur.Image.color = m_SliderColors[nextSliderIndex];
                            cur.SetData(elementMinValue, elementMaxValue);
                            curdelta.SetData(elementMinValue, elementMaxValue);
                        }

                        // 새로 세팅했으니 찾을 수 있을 것이다.
                        last = m_FrontElements.Last((element) => element.Value > 0);
                        deltaLast = m_DeltaElements.Last((element) => element.Value > 0);
                    }
                    finally
                    {
                        // 다시 또 재본다.
                        expectedValue = last.Value - delta;
                    }
                }


                if (last != null)
                {
                    last.Value = expectedValue;

                    // 누군가 이미 smoothValue를 하는 경우 마무리를 지어줘야 함
                    var find = m_DeltaElements.Find((element) => element.SmoothValueTasking);
                    if (find != null)
                        find.StopSmoothValue();

                    // 델타 변화
                    deltaLast.StartSmoothValue(expectedValue);
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

        SetData(0, 2000);
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

        // 백그라운드
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

        // 변화량
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_DeltaLayout.transform);
            inst.name = $"Delta Element ({i})";

            inst.Image.color = m_DeltaColor;
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
            m_DeltaElements.Add(inst);
        }

        // 실제 슬라이더
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_FrontLayout.transform);
            inst.name = $"Front Element ({i})";

            inst.Image.color = m_SliderColors[m_SliderColors.Length - 1];
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
            m_FrontElements.Add(inst);
        }

        Debug.LogWarning($"게임 시작 전에 {m_DeltaLayout}와 {m_FrontLayout}의 Horizontal Layout Group을 반드시 꺼주세요!");

        EditorUtility.SetDirty(this);
    }

    public void DestroyElements()
    {
        // 백그라운드
        for (int i = 0; i < m_BackElements.Count; i++)
        {
            var child = m_BackElements[i];
            DestroyImmediate(child.gameObject);
        }

        // 변화량
        for (int i = 0; i < m_DeltaElements.Count; i++)
        {
            var child = m_DeltaElements[i];
            DestroyImmediate(child.gameObject);
        }

        // 실제 슬라이더
        for (int i = 0; i < m_FrontElements.Count; i++)
        {
            var child = m_FrontElements[i];
            DestroyImmediate(child.gameObject);
        }

        m_BackElements.Clear();
        m_DeltaElements.Clear();
        m_FrontElements.Clear();
        m_CurrentColorIndex = 0;

        EditorUtility.SetDirty(this);
    }
}
