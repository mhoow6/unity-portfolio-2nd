using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InputSystem : MonoBehaviour, GameSystem
{
    public List<InputProvider> Controllers { get; private set; } = new List<InputProvider>();
    public Vector2 ControllerInput
    {
        get
        {
            if (m_MainController != null)
                return m_MainController.Input;
            else
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
                    StartCoroutine(m_Rotating);
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android)
                    StopCoroutine(m_Rotating);

                var cam = GameManager.Instance.MainCam.GetComponent<CinemachineBrain>();
                var activeCam = cam.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>();
                activeCam.m_XAxis.m_MaxSpeed = 0;
                activeCam.m_YAxis.m_MaxSpeed = 0;
            }     
        }
    }

    IEnumerator m_Rotating;
    const float ROTATE_BREAK_SENSTIVITY = 2f;
    InputProvider m_MainController;
    

    public void Init()
    {
        m_Rotating = RotateCoroutine();
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

    IEnumerator RotateCoroutine()
    {
        var cam = GameManager.Instance.MainCam.GetComponent<CinemachineBrain>();
        var activeCam = cam.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>();
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
