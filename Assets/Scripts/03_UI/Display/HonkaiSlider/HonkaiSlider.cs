using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HonkaiSlider : Display
{
    [Header("# 슬라이더 형태 결정요소")]
    [SerializeField] int SliceCount = 1;
    [SerializeField] List<Color> SliderColors = new List<Color>();

    [Header("# 에디터에서 가져오기")]
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

            // 얼만큼 줄었는지 Delta로 보여주기
        }
    }

    [SerializeField, ReadOnly] float m_Value;

    float m_MaxGauge;

    private void Awake()
    {
        // 예외처리
        if (SliderColors.Count < 2)
        {
            Debug.LogError("슬라이더의 색깔은 무조건 2개 이상이여야 합니다. (ex. 0:백그라운드로 놓을 색상, 1:슬라이더로 쓰일 색상)");
            enabled = false;
        }
        if (FramePrefab == null)
        {
            Debug.LogError("슬라이더로 쓰일 프레임 프리팹이 없습니다. 등록하고 사용해주세요.");
            enabled = false;
        }

        // 초기 세팅
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
        // UNDONE: 테스트
        SetData(1000);
    }

    public void SetData(int maxGauge)
    {
        m_MaxGauge = maxGauge;

        // 색깔에 따라 Back, Front 세팅
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
