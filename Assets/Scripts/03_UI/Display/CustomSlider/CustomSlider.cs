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
            m_Value = value;
            m_OnValueUpdate?.Invoke(value);
            
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
    
    public void SetData(float minValue, float maxValue, float currentValue, int sliderCount, Action<float> onValueUpdate = null)
    {
        
    }

    public void CreateElements()
    {
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
}
