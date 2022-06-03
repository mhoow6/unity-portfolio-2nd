using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : Display
{
    [SerializeField] int m_ElementCount;
    [SerializeField] int m_SliderCount;
    int m_PrevSliderCount;
    [SerializeField] Color m_BackgroundColor;
    [SerializeField] Color m_DeltaColor;
    [SerializeField] Color[] m_SliderColors;
    [SerializeField] HorizontalLayoutGroup m_BackgroundLayout;
    [SerializeField] HorizontalLayoutGroup m_DeltaLayout;
    [SerializeField] HorizontalLayoutGroup m_FrontLayout;
    [SerializeField] CustomSliderElement m_ElementPrefab;

    public void CreateElements()
    {
        // 백그라운드
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_BackgroundLayout.transform);
            inst.Image.color = m_BackgroundColor;
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
        }

        // 변화량
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_DeltaLayout.transform);
            inst.Image.color = m_DeltaColor;
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
        }

        // 실제 슬라이더
        for (int i = 0; i < m_ElementCount; i++)
        {
            var inst = Instantiate(m_ElementPrefab, m_FrontLayout.transform);
            inst.Image.color = m_SliderColors[m_SliderColors.Length - 1];
            inst.RectTransform.pivot = new Vector2(0, 0.5f);
        }
    }

    public void DestroyElements()
    {
        // 백그라운드
        for (int i = 0; i < m_BackgroundLayout.transform.childCount; i++)
        {
            var child = m_BackgroundLayout.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        // 변화량
        for (int i = 0; i < m_DeltaLayout.transform.childCount; i++)
        {
            var child = m_DeltaLayout.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        // 실제 슬라이더
        for (int i = 0; i < m_FrontLayout.transform.childCount; i++)
        {
            var child = m_FrontLayout.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }
}
