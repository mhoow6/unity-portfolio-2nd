using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InputSystem : MonoBehaviour, GameSystem
{
    public List<InputProvider> Controllers { get; private set; } = new List<InputProvider>();
    public Vector2 CharacterMoveInput
    {
        get
        {
            if (m_MainController != null)
                return m_MainController.Input;
            else
                return Vector2.zero;
        }
    }
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

    IEnumerator m_CameraRotate;
    const float ROTATE_BREAK_SENSTIVITY = 2f;
    InputProvider m_MainController;
    
    public void Init()
    {
        m_CameraRotate = CameraRotateCoroutine();
    }

    public void Tick()
    {
        // 메인 컨트롤러 감지
        foreach (var ctrl in Controllers)
        {
            if (ctrl.Input.magnitude != 0)
                m_MainController = ctrl;
        }
    }

    IEnumerator CameraRotateCoroutine()
    {
        var activeCam = StageManager.Instance.FreeLookCam;
        float XAxisMaxSpeed = activeCam.m_XAxis.m_MaxSpeed;
        float YAxisMaxSpeed = activeCam.m_YAxis.m_MaxSpeed;

        while (true)
        {
            if (Input.GetMouseButton(1))
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
}
