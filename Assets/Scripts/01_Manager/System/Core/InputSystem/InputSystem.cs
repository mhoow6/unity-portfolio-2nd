using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSystem : MonoBehaviour, IGameSystem, ISubscribable
{
    public List<InputProvider> Controllers { get; private set; } = new List<InputProvider>();
    public EventSystem EventSystem;

    #region 게임 입력
    InputProvider m_MainController;

    #region 스틱
    public Vector2 LeftStickInput
    {
        get
        {
            if (m_MainController != null)
                return m_MainController.Input;
            else
                return Vector2.zero;
        }
    }
    #endregion

    #region A 버튼
    public bool PressAButton
    {
        get
        {
            return m_PressAButton;
        }
        set
        {
            m_PressAButton = value;
            OnPressAButton?.Invoke(value);
        }
    }
    public Action<bool> OnPressAButton;
    public bool HoldAButton
    {
        get
        {
            return m_HoldAButton;
        }
        private set
        {
            m_HoldAButton = value;
            OnHoldAButton?.Invoke(value);
        }
    }
    public Action<bool> OnHoldAButton;

    bool m_PressAButton;
    bool m_HoldAButton;
    float m_InputATimer = 0f;
    #endregion

    #region X 버튼
    public bool PressXButton
    {
        get
        {
            return m_PressXButton;
        }
        set
        {
            m_PressXButton = value;
            OnPressXButton?.Invoke(value);
        }
    }
    public Action<bool> OnPressXButton;
    public bool HoldXButton
    {
        get
        {
            return m_HoldXButton;
        }
        private set
        {
            m_HoldXButton = value;
            OnHoldXButton?.Invoke(value);
        }
    }
    public Action<bool> OnHoldXButton;

    bool m_PressXButton;
    bool m_HoldXButton;
    float m_InputXTimer = 0f;
    #endregion

    #region B 버튼
    public bool PressBButton
    {
        get
        {
            return m_PressBButton;
        }
        set
        {
            m_PressBButton = value;
            OnPressBButton?.Invoke(value);
        }
    }
    public Action<bool> OnPressBButton;
    public bool HoldBButton
    {
        get
        {
            return m_HoldBButton;
        }
        private set
        {
            m_HoldBButton = value;
            OnHoldBButton?.Invoke(value);
        }
    }
    public Action<bool> OnHoldBButton;

    bool m_PressBButton;
    bool m_HoldBButton;
    float m_InputBTimer = 0f;
    #endregion

    #region Y 버튼
    public bool PressYButton
    {
        get
        {
            return m_PressYButton;
        }
        set
        {
            m_PressYButton = value;
            OnPressYButton?.Invoke(value);
        }
    }
    public Action<bool> OnPressYButton;
    public bool HoldYButton
    {
        get
        {
            return m_HoldYButton;
        }
        private set
        {
            m_HoldYButton = value;
            OnHoldYButton?.Invoke(value);
        }
    }
    public Action<bool> OnHoldYButton;

    bool m_PressYButton;
    bool m_HoldYButton;
    float m_InputYTimer = 0f;
    #endregion

    #endregion

    #region 카메라 입력
    public Vector2 CameraRotateInput
    {
        get
        {
            var cam = StageManager.Instance.FreeLookCam;
            if (cam != null)
                return new Vector2(cam.m_XAxis.m_InputAxisValue, cam.m_YAxis.m_InputAxisValue);
            return Vector2.zero;
        }
        set
        {
            var cam = StageManager.Instance.FreeLookCam;
            if (cam != null)
            {
                cam.m_XAxis.Value = value.x;
                cam.m_YAxis.Value = value.y;
            }    
        }
    }
    public bool CameraRotatable
    {
        set
        {
            if (value)
            {
#if !(UNITY_EDITOR)
                StartCoroutine(m_CameraRotate);
#endif
                var freelookCam = StageManager.Instance.FreeLookCam;
                if (freelookCam)
                {
                    freelookCam.m_XAxis.m_MaxSpeed = 300;
                    freelookCam.m_YAxis.m_MaxSpeed = 2;
                }
            }
            else
            {
#if !(UNITY_EDITOR)
                StopCoroutine(m_CameraRotate);
#endif
                var freelookCam = StageManager.Instance.FreeLookCam;
                if (freelookCam)
                {
                    freelookCam.m_XAxis.m_MaxSpeed = 0;
                    freelookCam.m_YAxis.m_MaxSpeed = 0;
                }
                
            }     
        }
    }

    float TouchSensitivity_x = 10f;
    float TouchSensitivity_y = 10f;

    // 외부에서 사용하는 포인터ID들. 카메라를 회전시킬 FingerId를 확실하게 식별하게 하는 용도
    [ReadOnly] public List<int> ExternallyUsingFingerIds = new List<int>();
    IEnumerator m_CameraRotate;
    [SerializeField] RectTransform m_CameraNoTouchArea;
    CustomRect m_CameraNoTouchRect;
#endregion

    public void Init()
    {
        m_CameraRotate = MobileCameraRotateCoroutine();

        // 터치 가능 영역 구하기
        float centerX = 250f;
        float centerY = 250f;

        float heightRatio = m_CameraNoTouchArea.rect.height / 1080;
        float widthRatio = m_CameraNoTouchArea.rect.width / 1920;
        float width = Screen.width * widthRatio;
        float height = Screen.height * heightRatio;

        Vector2 center = new Vector2(centerX, centerY);
        m_CameraNoTouchRect = new CustomRect(center, width, height);
        m_CameraNoTouchArea.gameObject.SetActive(false);

        // 이벤트 시스템은 항상 있어야해
        if (EventSystem != null)
            DontDestroyOnLoad(EventSystem);
    }

    public void Tick()
    {
        // 메인 컨트롤러 감지
        foreach (var ctrl in Controllers)
        {
            if (ctrl.Input.magnitude != 0)
            {
                if (!ctrl.Equals(m_MainController))
                {
                    Debug.Log($"현재 메인 컨트롤러: {ctrl.DeviceName}");
                    m_MainController = ctrl;
                }
            }
        }

        // 홀드 감지
        HoldAButton = DetectHoldInput(PressAButton, ref m_InputATimer);
        HoldXButton = DetectHoldInput(PressXButton, ref m_InputXTimer);
        HoldBButton = DetectHoldInput(PressBButton, ref m_InputBTimer);
        HoldYButton = DetectHoldInput(PressYButton, ref m_InputYTimer);
    }

    public void DisposeEvents()
    {
        OnPressAButton = null;
        OnHoldAButton = null;
        OnPressBButton = null;
        OnHoldBButton = null;
        OnPressXButton = null;
        OnHoldXButton = null;

        Debug.Log("인풋 시스템의 DisposeEvents");
    }

    IEnumerator MobileCameraRotateCoroutine()
    {
        var activeCam = StageManager.Instance.FreeLookCam;
        activeCam.m_XAxis.m_InputAxisName = string.Empty;
        activeCam.m_YAxis.m_InputAxisName = string.Empty;

        while (true)
        {
            // 카메라 터치 영역에 있는 손가락 찾기
            // 그런데 쓰고 있는 손가락이면 안 됨
            int fingerId = -1;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.touches[i];

                if (!m_CameraNoTouchRect.IsScreenPointInRect(touch.position) && !ExternallyUsingFingerIds.Contains(i))
                {
                    fingerId = i;
                    //Debug.Log($"{touch.phase}");
                    break;
                }
            }

            // 손가락의 델타포지션을 이용하여 카메라 회전하기
            if (Input.touchCount > 0 && fingerId != -1)
            {
                float deltaPosition_x = Input.touches[fingerId].deltaPosition.x;
                float deltaPosition_y = Input.touches[fingerId].deltaPosition.y;

                //Debug.Log($"deltaPosition.x ({fingerId}): {Input.touches[fingerId].deltaPosition.x}");
                //Debug.Log($"deltaPosition.y ({fingerId}): {Input.touches[fingerId].deltaPosition.y}");

                if (Mathf.Abs(deltaPosition_x) > Mathf.Abs(deltaPosition_y)) // 수평이동 값이 수직이동 값보다 클 경우
                    activeCam.m_XAxis.Value = Input.touches[fingerId].deltaPosition.x / TouchSensitivity_x;
                else if (Mathf.Abs(deltaPosition_x) < Mathf.Abs(deltaPosition_y))
                {
                    float value_y = deltaPosition_y / TouchSensitivity_y;

                    activeCam.m_YAxis.Value = Mathf.Lerp(activeCam.m_YAxis.Value, value_y, Time.deltaTime);
                }

                //Debug.Log($"xAxis.Value ({fingerId}): {activeCam.m_XAxis.Value}");
                //Debug.Log($"yAxis.Value ({fingerId}): {activeCam.m_YAxis.Value}");
            }
            

            yield return null;
        }
    }

    bool DetectHoldInput(bool inputSignal, ref float inputTimer)
    {
        bool result = false;

        if (inputSignal)
            inputTimer += Time.deltaTime;
        else
            inputTimer = 0f;

        if (inputTimer > 1f)
            result = true;
        else
            result = false;

        return result;
    }
}
