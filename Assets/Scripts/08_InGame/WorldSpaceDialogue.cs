using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DatabaseSystem;

public class WorldSpaceDialogue : MonoBehaviour
{
    [SerializeField] CinemachineBlenderSettings m_BlenderSettings;
    [SerializeField] CinemachineVirtualCamera m_DialogueLookCamera;
    List<StageDialogueTable> m_Dialogues = new List<StageDialogueTable>();
    GameObject m_ReturnCamera;

    const float m_WAIT_FOR_BLENDING_TIME = 1f;

    public void SetData(List<StageDialogueTable> dialogues)
    {
        // ��ȭ��� ����
        m_Dialogues = dialogues;

        // Blend ����
        SetBlendSetting(GameManager.Instance.BrainCam, GameManager.Instance.FreeLookCam);

        // ī�޶� ����
        StartCoroutine(BlendingCoroutine(BlendType.OutIn));
    }

    void SetBlendSetting(CinemachineBrain brain, ICinemachineCamera activeCam)
    {
        // ���� ������ ���� ī�޶�� ĳ���� ������ ����
        //GameManager.Instance.InputSystem.CameraRotatable = false;
        //GameManager.Instance.Player.Moveable = false;

        // ������ ������ BlenderSetting�� brain�� �����Ų��.
        brain.m_CustomBlends = m_BlenderSettings;

        // Ʈ���Ÿ� ��� ��ȭâ�� ���� Blend ����
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.OutIn].m_From = activeCam.VirtualCameraGameObject.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.OutIn].m_To = m_DialogueLookCamera.name;

        // ��ȭâ�� ������ ���ư��� Blend ����
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.InOut].m_From = m_DialogueLookCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.OutIn].m_To = activeCam.VirtualCameraGameObject.name;

        // ��ȭâ ���� �� ���� ī�޶�� ���ư�����
        m_ReturnCamera = activeCam.VirtualCameraGameObject;
    }

    IEnumerator BlendingCoroutine(BlendType type)
    {
        yield return new WaitForSeconds(m_WAIT_FOR_BLENDING_TIME);

        switch (type)
        {
            case BlendType.OutIn:
                // ���� ī�޶� ��Ȱ��ȭ
                m_ReturnCamera.SetActive(false);

                // ��ȭâ�� �ٶ󺸴� ī�޶� Ȱ��ȭ���� ����
                m_DialogueLookCamera.gameObject.SetActive(true);
                break;
            case BlendType.InOut:
                // ���� ī�޶� ��Ȱ��ȭ
                m_DialogueLookCamera.gameObject.SetActive(false);

                // ��ȭâ�� �ٶ󺸴� ī�޶� Ȱ��ȭ���� ����
                m_ReturnCamera.SetActive(true);
                break;
            default:
                break;
        }
        
    }

    void NextDialogue()
    {

    }

    enum BlendType
    {
        OutIn,
        InOut
    }
}
