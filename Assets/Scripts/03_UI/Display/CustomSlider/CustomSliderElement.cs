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
            // �̹��� ũ�� ���̱�
            float clamped = value - m_MinValue;

            float ratio = clamped / m_MaxValue;
            RectTransform.sizeDelta = new Vector2(m_OriginWidth * ratio, RectTransform.sizeDelta.y);

            m_Value = clamped;
        }
    }
    public bool SmoothValueTasking;

    [SerializeField] Animator m_Animator;
    [SerializeField, ReadOnly] float m_Value;
    float m_MinValue;
    float m_MaxValue;
    float m_OriginWidth;
    bool m_OriginWidthSet;

    const float VALUE_SPEED = 0.2f;

    public void SetData(float minValue, float maxValue, float value)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Value = value;

        if (!m_OriginWidthSet)
        {
            m_OriginWidth = RectTransform.sizeDelta.x;
            m_OriginWidthSet = true;
        }

        float ratio = m_Value / m_MaxValue;
        RectTransform.sizeDelta = new Vector2(m_OriginWidth * ratio, RectTransform.sizeDelta.y);
    }

    public void StartSmoothValue(float desired)
    {
        // �̹� SmoothValue �ϰ� ������ �װ��� �������Ѿ� �Ѵ�.
        if (SmoothValueTasking)
            StopSmoothValue();

        StartCoroutine(SmoothValueCoroutine(desired));
    }

    public void StopSmoothValue()
    {
        Value = m_Value;
        SmoothValueTasking = false;

        if (m_Animator != null)
            m_Animator.SetBool("OnDelta", false);

        StopAllCoroutines();
    }

    IEnumerator SmoothValueCoroutine(float desired)
    {
        float clamped = desired - m_MinValue;
        float desiredRatio = clamped / m_MaxValue;
        float prevRatio = m_Value / m_MaxValue;
        float controlRatio = prevRatio;

        SmoothValueTasking = true;

        // �̹� �����̴� ���� desired���� �Ǿ��ִ�.
        // �̹����� ������ �پ��� ��ó�� ���̰� ����
        m_Value = desired;

        if (m_Animator != null)
            m_Animator.SetBool("OnDelta", true);

        yield return new WaitForSeconds(2f);
        if (desiredRatio <= prevRatio)
        {
            while (desiredRatio < controlRatio)
            {
                controlRatio -= VALUE_SPEED * Time.deltaTime;

                RectTransform.sizeDelta = new Vector2(m_OriginWidth * controlRatio, RectTransform.sizeDelta.y);

                yield return null;
            }
        }
        else
        {
            while (desiredRatio > controlRatio)
            {
                controlRatio += VALUE_SPEED * Time.deltaTime;

                RectTransform.sizeDelta = new Vector2(m_OriginWidth * controlRatio, RectTransform.sizeDelta.y);

                yield return null;
            }
        }

        if (m_Animator != null)
            m_Animator.SetBool("OnDelta", false);

        SmoothValueTasking = false;
        Debug.Log($"SmoothValue ����");
    }
}
