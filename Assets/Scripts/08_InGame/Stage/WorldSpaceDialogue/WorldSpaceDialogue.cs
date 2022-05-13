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
        // 대화목록 세팅
        m_Dialogues = dialogues;

        // Blend 세팅
        SetBlendSetting(GameManager.Instance.BrainCam, GameManager.Instance.FreeLookCam);

        // 카메라 연출
        StartCoroutine(BlendingCoroutine(BlendType.OutIn, () => 
        {
            // 맨 처음 대화자 세팅
            if (m_Dialogues.Count > 0)
            {
                var dialogue = m_Dialogues[0];
                if (dialogue.IsLeft)
                    m_LeftSpeaker.Speak(dialogue, m_Dialogue);
                else
                    m_RightSpeaker.Speak(dialogue, m_Dialogue);

                // 클릭하면 대화 다 읽어짐.
                StartCoroutine(ClickForDialogueReadCoroutine());
            }
        }));      
    }

    void SetBlendSetting(CinemachineBrain brain, ICinemachineCamera activeCam)
    {
        // 예외 방지를 위해 카메라와 캐릭터 움직임 끄기
        //GameManager.Instance.InputSystem.CameraRotatable = false;
        //GameManager.Instance.Player.Moveable = false;

        // 사전에 세팅한 BlenderSetting을 brain에 적용시킨다.
        brain.m_CustomBlends = m_BlenderSettings;

        // 트리거를 밟아 대화창을 볼때 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.OutIn].m_From = activeCam.VirtualCameraGameObject.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.OutIn].m_To = m_DialogueLookCamera.name;

        // 대화창을 끝내고 돌아갈때 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.InOut].m_From = m_DialogueLookCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.InOut].m_To = activeCam.VirtualCameraGameObject.name;

        // 대화창 끝낼 때 원래 카메라로 돌아가야함
        m_ReturnCamera = activeCam.VirtualCameraGameObject;
    }

    IEnumerator BlendingCoroutine(BlendType type, Action blendDoneCallback = null)
    {
        yield return new WaitForSeconds(WAIT_FOR_BLENDING_TIME);
        GameObject activeCam = null;

        switch (type)
        {
            case BlendType.OutIn:
                // 현재 카메라 비활성화
                m_ReturnCamera.SetActive(false);

                // 대화창을 바라보는 카메라를 활성화시켜 블랜딩
                activeCam = m_DialogueLookCamera.gameObject;
                m_DialogueLookCamera.gameObject.SetActive(true);
                break;
            case BlendType.InOut:
                // 현재 카메라 비활성화
                m_DialogueLookCamera.gameObject.SetActive(false);

                // 대화창을 바라보는 카메라를 활성화시켜 블랜딩
                activeCam = m_ReturnCamera;
                m_ReturnCamera.SetActive(true);
                break;
            default:
                break;
        }

        // 활성화될 카메라가 정위치에 올때까지 기다리기
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
