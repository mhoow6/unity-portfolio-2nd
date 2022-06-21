using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System;

/// <summary> Version 1.0 </summary>
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
        m_DeltaLayout.gameObject.SetActive(false);

        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Value = Mathf.Clamp(currentValue, m_MinValue, m_MaxValue);
        m_OnValueUpdate = onValueUpdate;

        float elementMaxValue = m_MaxValue / m_ElementCount / m_SliderCount;
        float elementMinValue = m_MinValue / m_ElementCount / m_SliderCount;

        // 슬라이더 element 색상
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

        // 한 층의 슬라이더의 element에 적용할 value의 합
        // 나누기는 값 손실을 일으켜서 빼기로 표현
        float elementValueSum = (elementMaxValue * m_ElementCount) - (m_MaxValue - currentValue);

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
    /// Value 프로퍼티에서 필요한 함수. value를 적용시킬 마지막 element를 구합니다.
    /// </summary>
    /// <param name="diff">현재 슬라이더의 value랑, 슬라이더에 적용될 value와의 차이값</param>
    /// <returns></returns>
    (SplitSliderElement front, SplitSliderElement delta) TryGetValuedElements(float diff)
    {
        (SplitSliderElement, SplitSliderElement) result;
        result.Item1 = null;
        result.Item2 = null;

        // 게이지가 변화할 element 찾기
        try
        {
            if (diff > 0)
            {
                result.Item1 = m_FrontElements.Last(element => element.Value > 0);
                result.Item2 = m_DeltaElements.Last(element => element.Value > 0);
            }
            else
            {
                result.Item1 = m_FrontElements.Last(element => element.Value > 0 && element.Value < element.MaxValue);
                result.Item2 = m_DeltaElements.Last(element => element.Value > 0 && element.Value < element.MaxValue);
            }
        }
        // 더 이상 값을 조정할 element가 없을 경우
        catch (Exception)
        {
            // 값이 증가중인데, 남은 element들의 값이 0이여서 생기는 예외
            var zeros = m_FrontElements.FindAll(element => element.Value == 0);
            var deltaZeros = m_DeltaElements.FindAll(element => element.Value == 0);
            if (diff < 0 && zeros.Count > 0)
            {
                // 그냥 뒤에껄 이용하라고 해야한다.
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

                    // 다시 찾아보기
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