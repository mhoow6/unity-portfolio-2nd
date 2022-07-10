using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DatabaseSystem;
using System;
using DG.Tweening;

#region 사용 설명서
/*
 * 1. 프리팹
 * WorldSpaceDialogue 루트의 1단계 자식들은 전부 비활성화 상태로 나둔다.
 * WorldSpaceDialogue를 활성화시켰을때도 1단계 자식들이 활성화되지 않게 하기 위함.
 */
#endregion

public class WorldSpaceDialogue : MonoBehaviour
{
    [SerializeField] CinemachineBlenderSettings m_BlenderSettings;
    [SerializeField] CinemachineVirtualCamera m_StartBlendingCamera;
    [SerializeField] CinemachineVirtualCamera m_EndBlendingCamera;
    [SerializeField] GameObject m_EndBlendingPostProcessing;

    // 플레이어 방향 고정시키는 용도
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

    /// <summary> 어떤 대화자가 말하고 있는가? </summary>
    SpeakerPreset m_CurrentSpeaker;
    [SerializeField] Text m_DialogueText;

    const float DIALOUGE_WINDOW_TWEENING_SPEED = 0.3f;

    Action m_DialogueEndCallback;

    public void SetData(List<StageDialogueTable> dialogues, Action dialogueEndCallback = null)
    {
        // 대화목록 세팅
        m_Dialogues = dialogues;
        m_DialogueEndCallback = dialogueEndCallback;

        // Blend 세팅
        SetBlendSetting();

        // 캔버스 카메라 세팅
        m_Canvas.worldCamera = StageManager.Instance.MainCam;

        // UI 끄기
        GameManager.UISystem.HUD = false;

        // 카메라 연출
        var brain = StageManager.Instance.BrainCam;
        StartCoroutine(BlendingCoroutine(WorldSpaceDialogueBlendType.StartToEnd, () => 
        {
            Invoke("DialogueSetting", 0.5f);
        }));      
    }

    public void DialogueRead()
    {
        // 대화자가 세팅되기 전에 마우스 버튼을 누르는 경우 제외
        if (m_CurrentSpeaker == null)
            return;

        // 대사 1초는 보게하자
        if (m_CurrentSpeaker.SpeakingTime < 1f)
            return;

        // 말하는 사람의 대화를 전부 다 출력한다.
        if (m_CurrentSpeaker.IsSpeaking)
            m_CurrentSpeaker.SpeakComplete();
        else
        {
            // 남은 대화가 있는 경우
            if (m_CurrentDialogueIndex < m_Dialogues.Count)
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
            else
            {
                // 더 이상 진행할 대화가 없다면 대화 종료
                m_Monitor.transform.DOScaleY(0, DIALOUGE_WINDOW_TWEENING_SPEED);

                StartCoroutine(BlendingCoroutine(WorldSpaceDialogueBlendType.EndToStart,
                    () =>
                    {
                        gameObject.SetActive(false);

                        GameManager.InputSystem.CameraRotatable = true;
                        StageManager.Instance.Player.Moveable = true;

                    // UI 켜기
                    GameManager.UISystem.HUD = true;

                    // 인게임 UI 켜기
                    GameManager.UISystem.OpenWindow(UIType.InGame);

                        m_DialogueEndCallback?.Invoke();
                    }));
            }
        }
    }

    // ---------------------------------------------------------------

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            DialogueRead();
    }

    void SetBlendSetting()
    {
        var sm = StageManager.Instance;
        var brain = sm.BrainCam;

        // 예외 방지를 위해 카메라와 
        GameManager.InputSystem.CameraRotatable = false;
        // 시작할때 카메라 위치 조정하기
        sm.FreeLookCam.transform.SetPositionAndRotation(m_StartBlendingCamera.transform.position, m_StartBlendingCamera.transform.rotation);

        // 캐릭터 움직임 끄기
        sm.Player.Moveable = false;
        // 캐릭터 정위치
        sm.Player.CurrentCharacter.transform.SetPositionAndRotation(m_CharacterFixedTransform.position, m_CharacterFixedTransform.rotation);
        sm.Player.CurrentCharacter.TryAttachToFloor();

        // startBlendingCamera 위치를 씬의 카메라 위치랑 동일하게 하기
        //m_StartBlendingCamera.transform.position = sm.FreeLookCam.transform.position;
        //m_StartBlendingCamera.transform.rotation = sm.FreeLookCam.transform.rotation;

        // 사전에 세팅한 BlenderSetting을 brain에 적용시킨다.
        brain.m_CustomBlends = m_BlenderSettings;

        // 트리거를 밟아 시작 카메라에서 대화창을 보는 카메라의 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)WorldSpaceDialogueBlendType.StartToEnd].m_From = m_StartBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)WorldSpaceDialogueBlendType.StartToEnd].m_To = m_EndBlendingCamera.name;

        // 대화창을 끝내고 시작 카메라로 돌아갈때 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)WorldSpaceDialogueBlendType.EndToStart].m_From = m_EndBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)WorldSpaceDialogueBlendType.EndToStart].m_To = m_StartBlendingCamera.name;

        // 시작 카메라에서 플레이어 카메라로 돌아갈때 Blend 세팅
        brain.m_CustomBlends.m_CustomBlends[(int)WorldSpaceDialogueBlendType.StartToPlayer].m_From = m_StartBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)WorldSpaceDialogueBlendType.StartToPlayer].m_To = sm.FreeLookCam.name;
    }

    IEnumerator BlendingCoroutine(WorldSpaceDialogueBlendType type, Action blendDoneCallback = null)
    {
        var brain = StageManager.Instance.BrainCam;
        switch (type)
        {
            case WorldSpaceDialogueBlendType.StartToEnd:
                // 대화창 끝낼 때 원래 카메라로 돌아가야한다
                m_ReturnCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject;

                // 현재 카메라를 끄고
                brain.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
                // 블랜딩 시작 카메라 켜기
                m_StartBlendingCamera.gameObject.SetActive(true);

                // 조금 기다린 뒤,
                yield return new WaitForSeconds(1f);

                // 시작 카메라를 비활성화 하고
                m_StartBlendingCamera.gameObject.SetActive(false);

                // 대화창을 바라보는 카메라를 활성화시켜 블랜딩
                m_EndBlendingCamera.gameObject.SetActive(true);
                // 포스트 프로세싱도 같이 켜준다.
                m_EndBlendingPostProcessing.SetActive(true);
                break;
            case WorldSpaceDialogueBlendType.EndToStart:
                // 대화창을 바라보는 카메라 비활성화하고
                m_EndBlendingCamera.gameObject.SetActive(false);
                // 포스트 프로세싱도 같이 꺼주고
                m_EndBlendingPostProcessing.SetActive(false);

                // 블랜딩 시작 카메라 켜서 블랜딩
                m_StartBlendingCamera.gameObject.SetActive(true);

                // 블랜딩을 기다린 뒤,
                yield return new WaitUntil(() => !brain.IsBlending);

                // 블랜딩 시작 카메라를 끈 뒤에
                m_StartBlendingCamera.gameObject.SetActive(false);
                // 다시 원래 카메라를 키기
                m_ReturnCamera.SetActive(true);

                m_ReturnCamera = null;
                yield return null;
                break;
            default:
                break;
        }

        // 활성화될 카메라가 정위치에 올때까지 기다리기
        yield return new WaitUntil(() => !brain.IsBlending);
        blendDoneCallback?.Invoke();
    }

    /// <summary> 첫 대화자 세팅. SetData에서 Invoke으로 호출 </summary>
    void DialogueSetting()
    {
        // 대화창 키기
        m_Monitor.gameObject.SetActive(true);

        // 대화창 트위닝
        float desired = m_Monitor.transform.localScale.y;
        m_Monitor.transform.localScale = new Vector3(m_Monitor.transform.localScale.x, 0, m_Monitor.transform.localScale.z);
        m_Monitor.transform.DOScaleY(desired, DIALOUGE_WINDOW_TWEENING_SPEED);

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
        }
    }
}

enum WorldSpaceDialogueBlendType
{
    /// <summary> 블랜딩 시작 카메라부터 끝 카메라 /// </summary>
    StartToEnd,
    /// <summary> 블랜딩 끝 카메라부터 시작 카메라 /// </summary>
    EndToStart,
    /// <summary> 블랜딩 끝 카메라부터 플레이어 카메라 /// </summary>
    StartToPlayer
}