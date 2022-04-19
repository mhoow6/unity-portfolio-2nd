using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InputSystem : MonoBehaviour, GameSystem
{
    public Inputable Controller;
    public bool CameraRotatable
    {
        set
        {
            if (value)
                StartCoroutine(m_Rotating);
            else
                StopCoroutine(m_Rotating);
        }
    }

    IEnumerator m_Rotating;
    const float ROTATE_BREAK_SENSTIVITY = 2f;

    public void Init()
    {
        m_Rotating = RotateCoroutine();
    }

    public void Tick()
    {
        
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
