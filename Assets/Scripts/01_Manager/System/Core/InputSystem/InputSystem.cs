using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour, IGameSystem
{
    public List<InputProvider> Controllers { get; private set; } = new List<InputProvider>();

    #region 캐릭터
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
    public bool CharacterAttackInput;
    public bool IsHoldAttackInput { get; private set; }

    public bool CharacterDashInput;
    public bool IsHoldDashInput { get; private set; }

    public bool CharacterUltiInput;
    public bool IsHoldUltiInput { get; private set; }
    #endregion

    #region 카메라
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

    #endregion

    IEnumerator m_CameraRotate;
    InputProvider m_MainController;
    float m_AttackInputTimer = 0f;
    float m_DashInputTimer = 0f;
    float m_UltiInputTimer = 0f;
    const float ROTATE_BREAK_SENSTIVITY = 2f;
    
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

        // 홀드 감지
        DetectHoldInput(CharacterAttackInput, ref m_AttackInputTimer);
        DetectHoldInput(CharacterDashInput, ref m_DashInputTimer);
        DetectHoldInput(CharacterUltiInput, ref m_UltiInputTimer);
        if (IsHoldInput(ref m_AttackInputTimer, out bool holdAttack))
            IsHoldAttackInput = holdAttack;
        if (IsHoldInput(ref m_DashInputTimer, out bool holdDash))
            IsHoldDashInput = holdDash;
        if (IsHoldInput(ref m_UltiInputTimer, out bool holdUlti))
            IsHoldUltiInput = holdUlti;
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
}
