using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DatabaseSystem;
using System;
using UnityEditor;

public class WorldSpaceDialogue : MonoBehaviour
{
    [SerializeField] CinemachineBlenderSettings m_BlenderSettings;
    [SerializeField] CinemachineVirtualCamera m_StartBlendingCamera;
    [SerializeField] CinemachineVirtualCamera m_EndBlendingCamera;

    // 플레이어 방향 고정시키기
    [SerializeField] Transform m_CharacterFixedTransform;
    // 대화가 끝나고 돌아갈 플레이어 카메라
    GameObject m_ReturnCamera;

    List<StageDialogueTable> m_Dialogues = new List<StageDialogueTable>();
    int m_CurrentDialogueIndex;

    [Header("# 모니터 구성요소")]
    [SerializeField] GameObject m_Monitor;
    [SerializeField] Canvas m_Canvas;
    [SerializeField] SpeakerPreset m_LeftSpeaker;
    [SerializeField] SpeakerPreset m_RightSpeaker;
    // 어떤 Speaker가 말하고 있는가?
    SpeakerPreset m_CurrentSpeaker;
    [SerializeField] Text m_DialogueText;
    IEnumerator m_CheckingForDialogueReadCoroutine;

    const float WAIT_FOR_BLENDING_TIME = 1f;

    private void Awake()
    {
        m_CheckingForDialogueReadCoroutine = CheckingForDialogueReadCoroutine();
    }

    public void SetData(List<StageDialogueTable> dialogues)
    {
        // 대화목록 세팅
        m_Dialogues = dialogues;

        // Blend 세팅
        SetBlendSetting();

        // 캔버스 카메라 세팅
        m_Canvas.worldCamera = GameManager.Instance.MainCam;

        // UI 끄기
        GameManager.Instance.UISystem.HUD = false;

        // 카메라 연출
        var brain = GameManager.Instance.BrainCam;
        StartCoroutine(BlendingCoroutine(BlendType.StartToEnd, () => 
        {
            // 대화창 키기
            m_Monitor.gameObject.SetActive(true);

            // 맨 처음 대화자 세팅
            if (m_Dialogues.Count > 0)
            {
                m_CurrentDialogueIndex = 0;
                var dialogue = m_Dialogues[m_CurrentDialogueIndex++];
                if (dialogue.IsLeft)
                {
                    m_LeftSpeaker.Speak(dialogue, m_DialogueText);
                    m_CurrentSpeaker = m_LeftSpeaker;
                    m_RightSpeaker.Listen();
                }
                else
                {
                    m_RightSpeaker.Speak(dialogue, m_DialogueText);
                    m_CurrentSpeaker = m_RightSpeaker;
                    m_LeftSpeaker.Listen();
                }

                // 클릭하면 대화 다 읽어짐.
                StartCoroutine(m_CheckingForDialogueReadCoroutine);
            }
        }));      
    }

    void SetBlendSetting()
    {
        var brain = GameManager.Instance.BrainCam;

        // 예외 방지를 위해 카메라와 캐릭터 움직임 끄기
        GameManager.Instance.InputSystem.CameraRotatable = false;
        GameManager.Instance.Player.Moveable = false;
        // 캐릭터 정위치
        GameManager.Instance.Player.CurrentCharacter.transform.position = m_CharacterFixedTransform.position;
        GameManager.Instance.Player.CurrentCharacter.transform.rotation = m_CharacterFixedTransform.rotation;

        // 사전에 세팅한 BlenderSetting을 brain에 적용시킨다.
        brain.m_CustomBlends = m_BlenderSettings;

        // 트리거를 밟아 시작 카메라에서 대화창을 보는 카메라의 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToEnd].m_From = m_StartBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToEnd].m_To = m_EndBlendingCamera.name;

        // 대화창을 끝내고 시작 카메라로 돌아갈때 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.EndToStart].m_From = m_EndBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.EndToStart].m_To = m_StartBlendingCamera.name;

        // 시작 카메라에서 플레이어 카메라로 돌아갈때 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToPlayer].m_From = m_StartBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToPlayer].m_To = GameManager.Instance.FreeLookCam.name;
    }

    IEnumerator BlendingCoroutine(BlendType type, Action blendDoneCallback = null)
    {
        var brain = GameManager.Instance.BrainCam;
        switch (type)
        {
            case BlendType.StartToEnd:
                // 대화창 끝낼 때 원래 카메라로 돌아가야함
                m_ReturnCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject;

                // 현재 카메라를 끄고
                brain.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
                // 블랜딩 시작 카메라 켜기
                m_StartBlendingCamera.gameObject.SetActive(true);

                // 조금 기다린 뒤,
                yield return new WaitForSeconds(WAIT_FOR_BLENDING_TIME);

                // 시작 카메라를 비활성화 하고
                m_StartBlendingCamera.gameObject.SetActive(false);
                // 대화창을 바라보는 카메라를 활성화시켜 블랜딩
                m_EndBlendingCamera.gameObject.SetActive(true);
                break;
            case BlendType.EndToStart:
                // 대화창을 바라보는 카메라 비활성화하고
                m_EndBlendingCamera.gameObject.SetActive(false);
                // 블랜딩 시작 카메라 켜서 블랜딩
                m_StartBlendingCamera.gameObject.SetActive(true);

                // 블랜딩을 기다린 뒤,
                yield return new WaitUntil(() => !brain.IsBlending);

                // 블랜딩 시작 카메라를 끈 뒤에
                m_StartBlendingCamera.gameObject.SetActive(false);
                // 다시 원래 카메라를 키기
                m_ReturnCamera.SetActive(true);

                // 대화창 비활성화
                m_Monitor.gameObject.SetActive(false);
                yield return null;
                break;
            default:
                break;
        }

        // 활성화될 카메라가 정위치에 올때까지 기다리기
        yield return null;
        yield return new WaitUntil(() => !brain.IsBlending);
        blendDoneCallback?.Invoke();
    }

    IEnumerator CheckingForDialogueReadCoroutine()
    {
        // 1. 누가 말하고 있는지 가져오기
        while (true)
        {
            // 클릭을 하면
            if (Input.GetMouseButtonDown(0))
            {
                // 남은 대화가 있는 경우
                if (m_CurrentDialogueIndex < m_Dialogues.Count)
                {
                    // 말하는 사람의 대화를 전부 다 출력한다.
                    if (m_CurrentSpeaker.IsSpeaking)
                        m_CurrentSpeaker.SpeakComplete();
                    else
                    {
                        // 말하고 있는 사람이 없는 거면 대화자가 말을 다 끝낸상태
                        var listener = m_CurrentSpeaker == m_LeftSpeaker ? m_RightSpeaker : m_LeftSpeaker;

                        // 다음 대화가 만약에 말하던 사람이면
                        var currentDialogue = m_Dialogues[m_CurrentDialogueIndex++];
                        if (currentDialogue.NpcName == m_CurrentSpeaker.SpeakerName)
                        {
                            // 계속 말하게 하게 하고
                            m_CurrentSpeaker.Speak(currentDialogue, m_DialogueText);
                        }
                        else
                        {
                            // 그게 아니면 기존에 말한 사람은 듣는 상태로 바꾸고
                            m_CurrentSpeaker.Listen();

                            // 듣는 사람이 말을 해야 한다.
                            listener.Speak(currentDialogue, m_DialogueText);
                            m_CurrentSpeaker = listener;
                        }
                    }
                }
                else
                {
                    // 마지막 대화면 마저 말하게 하고
                    if (m_CurrentSpeaker.IsSpeaking)
                        m_CurrentSpeaker.SpeakComplete();
                    // 더 이상 진행할 대화가 없다면 대화 종료
                    else
                    {
                        StartCoroutine(BlendingCoroutine(BlendType.EndToStart,
                            () =>
                            {
                                gameObject.SetActive(false);

                                // 인풋 원상복귀
                                GameManager.Instance.InputSystem.CameraRotatable = true;
                                GameManager.Instance.Player.Moveable = true;

                                // UI 끄기
                                GameManager.Instance.UISystem.HUD = true;

                                // 인게임 UI 켜기
                                GameManager.Instance.UISystem.OpenWindow(UIType.InGame);
                            }));
                    }
                    
                }

            }

            yield return null;
        }
    }

    enum BlendType
    {
        /// <summary> 블랜딩 시작 카메라부터 끝 카메라 /// </summary>
        StartToEnd,
        /// <summary> 블랜딩 끝 카메라부터 시작 카메라 /// </summary>
        EndToStart,
        /// <summary> 블랜딩 끝 카메라부터 플레이어 카메라 /// </summary>
        StartToPlayer
    }
}