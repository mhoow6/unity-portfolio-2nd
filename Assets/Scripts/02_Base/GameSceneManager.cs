using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSceneManager : MonoBehaviour
{
    public Player Player;

    [Header("# 수동 기입")]
    [SerializeField] protected Camera m_MainCam;
    public SceneCode SceneCode;

    #region 카메라
    public Camera MainCam
    {
        get
        {
            return m_MainCam;
        }
        set
        {
            m_MainCam = value;
            // 메인카메라를 바꿀때 시네머신 Brain이 있는 경우 적용
            var brain = value.GetComponent<CinemachineBrain>();
            if (brain)
            {
                BrainCam = brain;
                // 메인카메라를 바꿀때 FreeLook이 있는 경우 적용

                if (brain.ActiveVirtualCamera != null)
                {
                    var freelook = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>();
                    if (freelook)
                        FreeLookCam = freelook;
                }
            }
        }
    }

    public CinemachineBrain BrainCam { get; private set; }

    public CinemachineFreeLook FreeLookCam { get; private set; }
    #endregion
}
