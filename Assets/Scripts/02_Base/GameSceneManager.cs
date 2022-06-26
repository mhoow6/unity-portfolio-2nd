using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSceneManager : MonoBehaviour
{
    public Player Player;

    [Header("# ���� ����")]
    [SerializeField] protected Camera m_MainCam;
    public SceneCode SceneCode;

    #region ī�޶�
    public Camera MainCam
    {
        get
        {
            return m_MainCam;
        }
        set
        {
            m_MainCam = value;
            // ����ī�޶� �ٲܶ� �ó׸ӽ� Brain�� �ִ� ��� ����
            var brain = value.GetComponent<CinemachineBrain>();
            if (brain)
            {
                BrainCam = brain;
                // ����ī�޶� �ٲܶ� FreeLook�� �ִ� ��� ����

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
