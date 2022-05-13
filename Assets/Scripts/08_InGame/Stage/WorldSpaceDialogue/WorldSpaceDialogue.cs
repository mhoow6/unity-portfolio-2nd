using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DatabaseSystem;
using System;

public class WorldSpaceDialogue : MonoBehaviour
{
    [SerializeField] CinemachineBlenderSettings m_BlenderSettings;
    [SerializeField] CinemachineVirtualCamera m_DialogueLookCamera;
    GameObject m_ReturnCamera;

    List<StageDialogueTable> m_Dialogues = new List<StageDialogueTable>();

    [SerializeField] SpeakerPreset m_LeftSpeaker;
    [SerializeField] SpeakerPreset m_RightSpeaker;
    [SerializeField] Text m_Dialogue;

    const float WAIT_FOR_BLENDING_TIME = 1f;

    public void SetData(List<StageDialogueTable> dialogues)
    {
        // ��ȭ��� ����
        m_Dialogues = dialogues;

        // Blend ����
        SetBlendSetting(GameManager.Instance.BrainCam, GameManager.Instance.FreeLookCam);

        // ī�޶� ����
        StartCoroutine(BlendingCoroutine(BlendType.OutIn, () => 
        {
            // �� ó�� ��ȭ�� ����
            if (m_Dialogues.Count > 0)
            {
                var dialogue = m_Dialogues[0];
                if (dialogue.IsLeft)
                    m_LeftSpeaker.Speak(dialogue, m_Dialogue);
                else
                    m_RightSpeaker.Speak(dialogue, m_Dialogue);

                // Ŭ���ϸ� ��ȭ �� �о���.
                StartCoroutine(ClickForDialogueReadCoroutine());
            }
        }));      
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
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.InOut].m_To = activeCam.VirtualCameraGameObject.name;

        // ��ȭâ ���� �� ���� ī�޶�� ���ư�����
        m_ReturnCamera = activeCam.VirtualCameraGameObject;
    }

    IEnumerator BlendingCoroutine(BlendType type, Action blendDoneCallback = null)
    {
        yield return new WaitForSeconds(WAIT_FOR_BLENDING_TIME);
        GameObject activeCam = null;

        switch (type)
        {
            case BlendType.OutIn:
                // ���� ī�޶� ��Ȱ��ȭ
                m_ReturnCamera.SetActive(false);

                // ��ȭâ�� �ٶ󺸴� ī�޶� Ȱ��ȭ���� ����
                activeCam = m_DialogueLookCamera.gameObject;
                m_DialogueLookCamera.gameObject.SetActive(true);
                break;
            case BlendType.InOut:
                // ���� ī�޶� ��Ȱ��ȭ
                m_DialogueLookCamera.gameObject.SetActive(false);

                // ��ȭâ�� �ٶ󺸴� ī�޶� Ȱ��ȭ���� ����
                activeCam = m_ReturnCamera;
                m_ReturnCamera.SetActive(true);
                break;
            default:
                break;
        }

        // Ȱ��ȭ�� ī�޶� ����ġ�� �ö����� ��ٸ���
        yield return new WaitUntil(() => activeCam.gameObject.activeSelf);
        blendDoneCallback?.Invoke();
    }

    IEnumerator ClickForDialogueReadCoroutine()
    {
        yield return null;
    }

    enum BlendType
    {
        OutIn,
        InOut
    }
}
