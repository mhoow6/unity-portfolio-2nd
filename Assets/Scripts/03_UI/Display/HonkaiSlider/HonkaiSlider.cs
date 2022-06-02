using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HonkaiSlider : Display
{
    [Header("# �����̴� ���� �������")]
    [SerializeField] int SliceCount = 1;
    [SerializeField] List<Color> SliderColors = new List<Color>();

    [Header("# �����Ϳ��� ��������")]
    [SerializeField] HorizontalLayoutGroup BackLayout;
    Stack<HonkaiSliderFrame> m_BackFrames = new Stack<HonkaiSliderFrame>();
    [SerializeField] HorizontalLayoutGroup DeltaLayout;
    Stack<HonkaiSliderFrame> m_DeltaFrames = new Stack<HonkaiSliderFrame>();
    [SerializeField] HorizontalLayoutGroup FrontLayout;
    Stack<HonkaiSliderFrame> m_FrontFrames = new Stack<HonkaiSliderFrame>();

    [SerializeField] Color m_DeltaFrameColor;
    [SerializeField] HonkaiSliderFrame FramePrefab;

    public float Value
    {
        get
        {
            return m_Value;
        }
        set
        {
            float delta = m_Value - value;

            // ��ŭ �پ����� Delta�� �����ֱ�
        }
    }

    [SerializeField, ReadOnly] float m_Value;

    float m_MaxGauge;

    private void Awake()
    {
        // ����ó��
        if (SliderColors.Count < 2)
        {
            Debug.LogError("�����̴��� ������ ������ 2�� �̻��̿��� �մϴ�. (ex. 0:��׶���� ���� ����, 1:�����̴��� ���� ����)");
            enabled = false;
        }
        if (FramePrefab == null)
        {
            Debug.LogError("�����̴��� ���� ������ �������� �����ϴ�. ����ϰ� ������ּ���.");
            enabled = false;
        }

        // �ʱ� ����
        for (int i = 0; i < FrontLayout.transform.childCount; i++)
        {
            var child = FrontLayout.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < DeltaLayout.transform.childCount; i++)
        {
            var child = DeltaLayout.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < BackLayout.transform.childCount; i++)
        {
            var child = BackLayout.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        FrontLayout.enabled = true;
        DeltaLayout.enabled = true;
        BackLayout.enabled = true;
    }

    private void Start()
    {
        // UNDONE: �׽�Ʈ
        SetData(1000);
    }

    public void SetData(int maxGauge)
    {
        m_MaxGauge = maxGauge;

        // ���� ���� Back, Front ����
        int frontLayoutIndex = SliderColors.Count - 1;
        float gauge = (m_MaxGauge*0.5f) / SliceCount;
        for (int i = 0; i < SliceCount; i++)
        {
            var frame = Instantiate(FramePrefab, FrontLayout.transform);
            frame.SetData(SliderColors[frontLayoutIndex], gauge);
            m_FrontFrames.Push(frame);
        }
        for (int i = 0; i < SliceCount; i++)
        {
            var frame = Instantiate(FramePrefab, DeltaLayout.transform);
            frame.SetData(m_DeltaFrameColor, gauge);
            m_DeltaFrames.Push(frame);
        }
        for (int i = 0; i < SliceCount; i++)
        {
            var frame = Instantiate(FramePrefab, BackLayout.transform);
            frame.SetData(SliderColors[0], gauge);
            m_BackFrames.Push(frame);
        }

        FrontLayout.enabled = false;
        DeltaLayout.enabled = false;
        BackLayout.enabled = true;
    }
}
