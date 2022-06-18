using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSystem : MonoBehaviour, IGameSystem, IEventCallable
{
    public List<InputProvider> Controllers { get; private set; } = new List<InputProvider>();

    #region 게임 입력
    public Vector2 CharacterMoveInput
    {
        get
        {
            if (m_CharacterController != null)
                return m_CharacterController.Input;
            else
                return Vector2.zero;
        }
    }
    InputProvider m_CharacterController;

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
    bool m_PressAButton;
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
    bool m_HoldAButton;
    public Action<bool> OnHoldAButton;
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
    bool m_PressXButton;
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
    bool m_HoldXButton;
    public Action<bool> OnHoldXButton;
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
    bool m_PressBButton;
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

    bool m_HoldBButton;
    public Action<bool> OnHoldBButton;
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
    }

    public bool CameraRotatable
    {
        set
        {
            if (value)
            {
                if (Application.platform == RuntimePlatform.Android)
                    StartCoroutine(m_CameraRotate);

                var freelookCam = StageManager.Instance.FreeLookCam;
                if (freelookCam)
                {
                    freelookCam.m_XAxis.m_MaxSpeed = 300;
                    freelookCam.m_YAxis.m_MaxSpeed = 2;
                }
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android)
                    StopCoroutine(m_CameraRotate);

                var freelookCam = StageManager.Instance.FreeLookCam;
                if (freelookCam)
                {
                    freelookCam.m_XAxis.m_MaxSpeed = 0;
                    freelookCam.m_YAxis.m_MaxSpeed = 0;
                }
                
            }     
        }
    }
    public float TouchSensitivity_x = 10f;
    public float TouchSensitivity_y = 10f;

    IEnumerator m_CameraRotate;
    [SerializeField] RectTransform m_CameraTouchRectTransform;
    CustomRect m_CameraTouchRect;

    float m_AttackInputTimer = 0f;
    float m_DashInputTimer = 0f;
    float m_UltiInputTimer = 0f;
    const float ROTATE_BREAK_SENSTIVITY = 2f;
    #endregion

    public void Init()
    {
        m_CameraRotate = MobileCameraRotateCoroutine();

        // m_CameraTouchRect의 중앙 구하기
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;
        float height = m_CameraTouchRectTransform.rect.height;
        float width = m_CameraTouchRectTransform.rect.width;
        centerY += (height * 0.5f);
        Vector2 center = new Vector2(centerX, centerY);
        m_CameraTouchRect = new CustomRect(center, width, height);
        m_CameraTouchRectTransform.gameObject.SetActive(false);
    }

    public void Tick()
    {
        // 메인 컨트롤러 감지
        foreach (var ctrl in Controllers)
        {
            if (ctrl.Input.magnitude != 0)
            {
                if (!ctrl.Equals(m_CharacterController))
                {
                    Debug.Log($"현재 캐릭터 컨트롤러: {ctrl.DeviceName}");
                    m_CharacterController = ctrl;
                }
            }
        }

        // 홀드 감지
        DetectHoldInput(PressAButton, ref m_AttackInputTimer);
        DetectHoldInput(PressXButton, ref m_DashInputTimer);
        DetectHoldInput(PressBButton, ref m_UltiInputTimer);
        if (IsHoldInput(ref m_AttackInputTimer, out bool holdAttack))
            HoldAButton = holdAttack;
        if (IsHoldInput(ref m_DashInputTimer, out bool holdDash))
            HoldXButton = holdDash;
        if (IsHoldInput(ref m_UltiInputTimer, out bool holdUlti))
            HoldBButton = holdUlti;
    }

    public void DisposeEvents()
    {
        OnPressAButton = null;
        OnHoldAButton = null;
        OnPressBButton = null;
        OnHoldBButton = null;
        OnPressXButton = null;
        OnHoldXButton = null;
    }

    IEnumerator MobileCameraRotateCoroutine()
    {
        var activeCam = StageManager.Instance.FreeLookCam;
        float XAxisMaxSpeed = activeCam.m_XAxis.m_MaxSpeed;
        float YAxisMaxSpeed = activeCam.m_YAxis.m_MaxSpeed;

        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                activeCam.m_XAxis.m_MaxSpeed = XAxisMaxSpeed;
                activeCam.m_YAxis.m_MaxSpeed = YAxisMaxSpeed;
            }
            else
            {
                activeCam.m_XAxis.m_MaxSpeed = Mathf.Lerp(activeCam.m_XAxis.m_MaxSpeed, 0, ROTATE_BREAK_SENSTIVITY * Time.deltaTime);
                activeCam.m_YAxis.m_MaxSpeed = Mathf.Lerp(activeCam.m_YAxis.m_MaxSpeed, 0, ROTATE_BREAK_SENSTIVITY * Time.deltaTime);
            }
            yield return null;
        }
    }

    void DetectHoldInput(bool signal, ref float timer)
    {
        if (signal)
            timer += Time.deltaTime;
        else
            timer = 0f;
    }

    bool IsHoldInput(ref float timer, out bool result)
    {
        if (timer > 1f)
        {
            result = true;
            return true;
        }
        result = false;
        return false;
    }

    float HandleAxisInputDelegate(string axisName)
    {
        switch (axisName)
        {
            case "Mouse X":

                if (Input.touchCount > 0)
                {
                    return Input.touches[0].deltaPosition.x / TouchSensitivity_x;
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            case "Mouse Y":
                if (Input.touchCount > 0)
                {
                    return Input.touches[0].deltaPosition.y / TouchSensitivity_y;
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            default:
                Debug.LogError("Input <" + axisName + "> not recognyzed.", this);
                break;
        }

        return 0f;
    }
}
