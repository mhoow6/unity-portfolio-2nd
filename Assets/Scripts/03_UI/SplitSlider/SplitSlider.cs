using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System;

/// <summary> Version 1.01 </summary>
public class SplitSlider : Display
{
    [Header("# 조절 가능")]
    [SerializeField] int m_ElementCount;
    [SerializeField] Color m_BackgroundColor;
    [SerializeField] Color m_DeltaColor;
    [SerializeField] Color[] m_SliderColors;

    [Header("# 개발자 옵션")]
    [SerializeField, ReadOnly] int m_SliderCount;
    [SerializeField, ReadOnly] int m_CurrentColorIndex;
    [SerializeField] SplitSliderElement m_ElementPrefab;
    [SerializeField, ReadOnly] float m_Value;

    [Space(10)]
    [SerializeField] HorizontalLayoutGroup m_BackgroundLayout;
    [SerializeField, ReadOnly] List<SplitSliderElement> m_BackElements = new List<SplitSliderElement>();

    [SerializeField] HorizontalLayoutGroup m_DeltaLayout;
    [SerializeField, ReadOnly] List<SplitSliderElement> m_DeltaElements = new List<SplitSliderElement>();

    [SerializeField] HorizontalLayoutGroup m_FrontLayout;
    [SerializeField, ReadOnly] List<SplitSliderElement> m_FrontElements = new List<SplitSliderElement>();

    public float Value
    {
        get
        {
            return m_Value;
        }
        set
        {
            float clamped = Mathf.Clamp(value, m_MinValue, m_MaxValue);
            SplitSliderElement last = null;
            SplitSliderElement deltaLast = null;

            Queue<float> deltaQueue = new Queue<float>();

            float delta = m_Value - clamped;
            deltaQueue.Enqueue(delta);
            while (deltaQueue.Count != 0)
            {
                (last, deltaLast) = TryGetElements(delta >= 0);
                if (last != null)
                {
                    if (last.Value - delta >= m_ElementMinValue)
                    {
                        // element max value를 넘어서 증가하는 경우
                        if (last.Value - delta > m_ElementMaxValue)
                        {
                            delta += (m_ElementMaxValue - last.Value);

                            last.Value = m_ElementMaxValue;

                            deltaQueue.Dequeue();
                            deltaQueue.Enqueue(delta);
                        }
                        else
                        {
                            last.Value = last.Value - delta;
                            deltaQueue.Dequeue();
                        }
                    }
                    else if (last.Value - delta < m_ElementMinValue)
                    {
                        delta -= (last.Value - m_ElementMinValue);

                        last.Value = m_ElementMinValue;

                        deltaQueue.Dequeue();
                        deltaQueue.Enqueue(delta);
                    }

                }
                else
                    break;
            }

            m_Value = clamped;
            m_OnValueUpdate?.Invoke(clamped);
        }
    }
    public float MaxValue => m_MaxValue;
    public float MinValue => m_MinValue;

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
    float m_ElementMinValue;
    float m_ElementMaxValue;

    public void SetData(float minValue, float maxValue, Action<float> onValueUpdate = null)
    {
        m_DeltaLayout.gameObject.SetActive(false);

        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Value = maxValue;
        m_OnValueUpdate = onValueUpdate;

        float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;
        float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
        m_ElementMinValue = elementMinValue;
        m_ElementMaxValue = elementMaxValue;

        // 슬라이더 element 색상
        int colorIndex = m_SliderCount - 1;
        for (int i = 0; i < m_SliderCount; i++)
        {
            float minRange = (m_MaxValue / m_SliderCount) * i;
            float maxRange = (m_MaxValue / m_SliderCount) * (i + 1);

            if (m_Value >= minRange && m_Value < maxRange)
            {
                colorIndex = i;
                break;
            }
        }
        m_CurrentColorIndex = colorIndex;

        int backgroundIndex = colorIndex - 1;
        for (int i = 0; i < m_ElementCount; i++)
        {
            // 백그라운드 색상
            var back = m_BackElements[i];
            if (backgroundIndex >= 0)
                back.Image.color = m_SliderColors[backgroundIndex];
            else
                back.Image.color = m_BackgroundColor;

            // 프론트 색상
            var front = m_FrontElements[i];
            front.Image.color = m_SliderColors[m_CurrentColorIndex];
        }

        float elementValueSum = m_ElementMaxValue * m_ElementCount;
        // 슬라이더 element 값
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

#if UNITY_EDITOR
    public void CreateElements()
    {
        if (m_BackElements.Count > 0)
        {
            Debug.LogError($"기존의 element를 삭제하고 해주세요.");
            return;
        }

        if (Application.isPlaying)
        {
            Debug.LogError($"게임 실행중에 슬라이더의 모양을 변경시킬 수 없습니다. 에디터 모드에서 실행해주세요.");
            return;
        }

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
        m_SliderCount = m_SliderColors.Length;
        m_CurrentColorIndex = m_SliderColors.Length - 1;

        Debug.LogWarning(
            $"게임 시작 전에 {m_DeltaLayout}와 {m_FrontLayout}의 Horizontal Layout Group을 반드시 꺼주세요!\n" +
            $"현재 버젼에는 슬라이더 값 변화량 연출이 구현이 되어있지 않아. {m_DeltaLayout.name}을 비활성화합니다.");
        m_DeltaLayout.gameObject.SetActive(false);

        EditorUtility.SetDirty(this);
    }

    public void DestroyElements()
    {
        if (Application.isPlaying)
        {
            Debug.LogError($"게임 실행중에 슬라이더의 모양을 변경시킬 수 없습니다. 에디터 모드에서 실행해주세요.");
            return;
        }

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

        m_SliderCount = 0;
        m_CurrentColorIndex = 0;

        EditorUtility.SetDirty(this);
    }
#endif

    /// <summary>
    /// Value 프로퍼티에서 필요한 함수. 값을 적용시킬 element를 구합니다.
    /// </summary>
    /// <param name="decresing">감소하는 중이거나 변화가 없으면 true 아니면 false</param>
    /// <returns></returns>
    (SplitSliderElement front, SplitSliderElement delta) TryGetElements(bool decresing)
    {
        (SplitSliderElement, SplitSliderElement) result;
        result.Item1 = null;
        result.Item2 = null;

        // 게이지가 변화할 element 찾기
        if (decresing)
        {
            result.Item1 = m_FrontElements.LastOrDefault(element => element.Value > m_ElementMinValue);
            result.Item2 = m_DeltaElements.LastOrDefault(element => element.Value > m_ElementMinValue);
        }
        else
        {
            result.Item1 = m_FrontElements.FirstOrDefault(element => element.Value < m_ElementMaxValue);
            result.Item2 = m_DeltaElements.FirstOrDefault(element => element.Value < m_ElementMaxValue);
        }

        // 더 이상 값을 조정할 element가 없을 경우
        if (result.Item1 == null)
        {
            int nextSliderIndex = decresing ? --m_CurrentColorIndex : ++m_CurrentColorIndex;

            // 새로운 색상의 슬라이더를 세팅한다.
            if (nextSliderIndex >= 0)
            {
                for (int i = 0; i < m_ElementCount; i++)
                {
                    var cur = m_FrontElements[i];
                    var curdelta = m_DeltaElements[i];
                    var back = m_BackElements[i];

                    // 백그라운드 색상 초기화
                    int backgroundIndex = nextSliderIndex - 1;
                    if (backgroundIndex >= 0)
                        back.Image.color = m_SliderColors[backgroundIndex];
                    else
                        back.Image.color = m_BackgroundColor;

                    // element 색상 초기화
                    float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
                    float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;
                    cur.Image.color = m_SliderColors[nextSliderIndex];

                    // element 값 새로 세팅
                    if (decresing)
                    {
                        cur.SetData(elementMinValue, elementMaxValue, elementMaxValue);
                        curdelta.SetData(elementMinValue, elementMaxValue, elementMaxValue);
                    }
                    else
                    {
                        cur.SetData(elementMinValue, elementMaxValue, elementMinValue);
                        curdelta.SetData(elementMinValue, elementMaxValue, elementMinValue);
                    }
                }

                // 다시 찾아보기
                if (decresing)
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

        return result;
    }
}