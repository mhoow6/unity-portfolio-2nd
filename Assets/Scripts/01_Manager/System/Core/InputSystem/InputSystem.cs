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

    #region ���� �Է�
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

    #region A ��ư
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
    float m_AttackInputTimer = 0f;
    #endregion

    #region X ��ư
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
    float m_DashInputTimer = 0f;
    #endregion

    #region B ��ư
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
    float m_UltiInputTimer = 0f;
    #endregion

    #endregion

    #region ī�޶� �Է�
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

    float TouchSensitivity_x = 10f;
    float TouchSensitivity_y = 10f;

    // �ܺο��� ����ϴ� ������ID��. ī�޶� ȸ����ų FingerId�� Ȯ���ϰ� �ĺ��ϰ� �ϴ� �뵵
    [ReadOnly] public List<int> ExternallyUsingFingerIds = new List<int>();
    IEnumerator m_CameraRotate;
    [SerializeField] RectTransform m_CameraTouchRectTransform;
    CustomRect m_CameraTouchRect;
    #endregion

    public void Init()
    {
        m_CameraRotate = MobileCameraRotateCoroutine();

        // m_CameraTouchRect�� �߾� ���ϱ�
        float centerX = Screen.width * 0.5f;
        float centerY = Screen.height * 0.5f;

        float heightRatio = m_CameraTouchRectTransform.rect.height / 1080;
        float widthRatio = m_CameraTouchRectTransform.rect.width / 1920;
        float width = Screen.width * widthRatio;
        float height = Screen.height * heightRatio;

        centerY += m_CameraTouchRectTransform.anchoredPosition.y;

        Vector2 center = new Vector2(centerX, centerY);
        m_CameraTouchRect = new CustomRect(center, width, height);
        m_CameraTouchRectTransform.gameObject.SetActive(false);

        if (EventSystem != null)
            DontDestroyOnLoad(EventSystem);
    }

    public void Tick()
    {
        // ���� ��Ʈ�ѷ� ����
        foreach (var ctrl in Controllers)
        {
            if (ctrl.Input.magnitude != 0)
            {
                if (!ctrl.Equals(m_CharacterController))
                {
                    Debug.Log($"���� ĳ���� ��Ʈ�ѷ�: {ctrl.DeviceName}");
                    m_CharacterController = ctrl;
                }
            }
        }

        // Ȧ�� ����
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
        activeCam.m_XAxis.m_InputAxisName = string.Empty;
        activeCam.m_YAxis.m_InputAxisName = string.Empty;

        while (true)
        {
            // ī�޶� ��ġ ������ �ִ� �հ��� ã��
            // �׷��� ���� �ִ� �հ����̸� �� ��
            int fingerId = -1;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.touches[i];

                if (m_CameraTouchRect.IsScreenPointInRect(touch.position) && !ExternallyUsingFingerIds.Contains(i))
                {
                    fingerId = i;
                    Debug.Log($"{touch.phase}");
                    break;
                }
            }

            // �հ����� ��Ÿ�������� �̿��Ͽ� ī�޶� ȸ���ϱ�
            if (Input.touchCount > 0 && fingerId != -1)
            {
                Debug.Log($"deltaPosition.x ({fingerId}): {Input.touches[fingerId].deltaPosition.x}");
                Debug.Log($"deltaPosition.y ({fingerId}): {Input.touches[fingerId].deltaPosition.y}");

                if (Mathf.Abs(Input.touches[fingerId].deltaPosition.x) > Mathf.Abs(Input.touches[fingerId].deltaPosition.y)) // �����̵� ���� �����̵� ������ Ŭ ���
                    activeCam.m_XAxis.Value = Input.touches[fingerId].deltaPosition.x / TouchSensitivity_x;
                else if (Mathf.Abs(Input.touches[fingerId].deltaPosition.x) < Mathf.Abs(Input.touches[fingerId].deltaPosition.y))
                {
                    float value_y = Input.touches[fingerId].deltaPosition.y / TouchSensitivity_y;

                    activeCam.m_YAxis.Value = Mathf.Lerp(activeCam.m_YAxis.Value, value_y, Time.deltaTime);
                }
                    

                Debug.Log($"xAxis.Value ({fingerId}): {activeCam.m_XAxis.Value}");
                Debug.Log($"yAxis.Value({fingerId}): {activeCam.m_YAxis.Value}");
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
        // Start() => GetInputAxis HandleAxisInputDelegate
        //CinemachineCore.GetInputAxis = HandleAxisInputDelegate;

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
