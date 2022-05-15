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

    // �÷��̾� ���� ������Ű��
    [SerializeField] Transform m_CharacterFixedTransform;
    // ��ȭ�� ������ ���ư� �÷��̾� ī�޶�
    GameObject m_ReturnCamera;

    List<StageDialogueTable> m_Dialogues = new List<StageDialogueTable>();
    int m_CurrentDialogueIndex;

    [Header("# ����� �������")]
    [SerializeField] GameObject m_Monitor;
    [SerializeField] Canvas m_Canvas;
    [SerializeField] SpeakerPreset m_LeftSpeaker;
    [SerializeField] SpeakerPreset m_RightSpeaker;
    // � Speaker�� ���ϰ� �ִ°�?
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
        // ��ȭ��� ����
        m_Dialogues = dialogues;

        // Blend ����
        SetBlendSetting();

        // ĵ���� ī�޶� ����
        m_Canvas.worldCamera = GameManager.Instance.MainCam;

        // UI ����
        GameManager.Instance.UISystem.HUD = false;

        // ī�޶� ����
        var brain = GameManager.Instance.BrainCam;
        StartCoroutine(BlendingCoroutine(BlendType.StartToEnd, () => 
        {
            // ��ȭâ Ű��
            m_Monitor.gameObject.SetActive(true);

            // �� ó�� ��ȭ�� ����
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

                // Ŭ���ϸ� ��ȭ �� �о���.
                StartCoroutine(m_CheckingForDialogueReadCoroutine);
            }
        }));      
    }

    void SetBlendSetting()
    {
        var brain = GameManager.Instance.BrainCam;

        // ���� ������ ���� ī�޶�� ĳ���� ������ ����
        GameManager.Instance.InputSystem.CameraRotatable = false;
        GameManager.Instance.Player.Moveable = false;
        // ĳ���� ����ġ
        GameManager.Instance.Player.CurrentCharacter.transform.position = m_CharacterFixedTransform.position;
        GameManager.Instance.Player.CurrentCharacter.transform.rotation = m_CharacterFixedTransform.rotation;

        // ������ ������ BlenderSetting�� brain�� �����Ų��.
        brain.m_CustomBlends = m_BlenderSettings;

        // Ʈ���Ÿ� ��� ���� ī�޶󿡼� ��ȭâ�� ���� ī�޶��� Blend ����
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToEnd].m_From = m_StartBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToEnd].m_To = m_EndBlendingCamera.name;

        // ��ȭâ�� ������ ���� ī�޶�� ���ư��� Blend ����
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.EndToStart].m_From = m_EndBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.EndToStart].m_To = m_StartBlendingCamera.name;

        // ���� ī�޶󿡼� �÷��̾� ī�޶�� ���ư��� Blend ����
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToPlayer].m_From = m_StartBlendingCamera.name;
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToPlayer].m_To = GameManager.Instance.FreeLookCam.name;
    }

    IEnumerator BlendingCoroutine(BlendType type, Action blendDoneCallback = null)
    {
        var brain = GameManager.Instance.BrainCam;
        switch (type)
        {
            case BlendType.StartToEnd:
                // ��ȭâ ���� �� ���� ī�޶�� ���ư�����
                m_ReturnCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject;

                // ���� ī�޶� ����
                brain.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
                // ���� ���� ī�޶� �ѱ�
                m_StartBlendingCamera.gameObject.SetActive(true);

                // ���� ��ٸ� ��,
                yield return new WaitForSeconds(WAIT_FOR_BLENDING_TIME);

                // ���� ī�޶� ��Ȱ��ȭ �ϰ�
                m_StartBlendingCamera.gameObject.SetActive(false);
                // ��ȭâ�� �ٶ󺸴� ī�޶� Ȱ��ȭ���� ����
                m_EndBlendingCamera.gameObject.SetActive(true);
                break;
            case BlendType.EndToStart:
                // ��ȭâ�� �ٶ󺸴� ī�޶� ��Ȱ��ȭ�ϰ�
                m_EndBlendingCamera.gameObject.SetActive(false);
                // ���� ���� ī�޶� �Ѽ� ����
                m_StartBlendingCamera.gameObject.SetActive(true);

                // ������ ��ٸ� ��,
                yield return new WaitUntil(() => !brain.IsBlending);

                // ���� ���� ī�޶� �� �ڿ�
                m_StartBlendingCamera.gameObject.SetActive(false);
                // �ٽ� ���� ī�޶� Ű��
                m_ReturnCamera.SetActive(true);

                // ��ȭâ ��Ȱ��ȭ
                m_Monitor.gameObject.SetActive(false);
                yield return null;
                break;
            default:
                break;
        }

        // Ȱ��ȭ�� ī�޶� ����ġ�� �ö����� ��ٸ���
        yield return null;
        yield return new WaitUntil(() => !brain.IsBlending);
        blendDoneCallback?.Invoke();
    }

    IEnumerator CheckingForDialogueReadCoroutine()
    {
        // 1. ���� ���ϰ� �ִ��� ��������
        while (true)
        {
            // Ŭ���� �ϸ�
            if (Input.GetMouseButtonDown(0))
            {
                // ���� ��ȭ�� �ִ� ���
                if (m_CurrentDialogueIndex < m_Dialogues.Count)
                {
                    // ���ϴ� ����� ��ȭ�� ���� �� ����Ѵ�.
                    if (m_CurrentSpeaker.IsSpeaking)
                        m_CurrentSpeaker.SpeakComplete();
                    else
                    {
                        // ���ϰ� �ִ� ����� ���� �Ÿ� ��ȭ�ڰ� ���� �� ��������
                        var listener = m_CurrentSpeaker == m_LeftSpeaker ? m_RightSpeaker : m_LeftSpeaker;

                        // ���� ��ȭ�� ���࿡ ���ϴ� ����̸�
                        var currentDialogue = m_Dialogues[m_CurrentDialogueIndex++];
                        if (currentDialogue.NpcName == m_CurrentSpeaker.SpeakerName)
                        {
                            // ��� ���ϰ� �ϰ� �ϰ�
                            m_CurrentSpeaker.Speak(currentDialogue, m_DialogueText);
                        }
                        else
                        {
                            // �װ� �ƴϸ� ������ ���� ����� ��� ���·� �ٲٰ�
                            m_CurrentSpeaker.Listen();

                            // ��� ����� ���� �ؾ� �Ѵ�.
                            listener.Speak(currentDialogue, m_DialogueText);
                            m_CurrentSpeaker = listener;
                        }
                    }
                }
                else
                {
                    // ������ ��ȭ�� ���� ���ϰ� �ϰ�
                    if (m_CurrentSpeaker.IsSpeaking)
                        m_CurrentSpeaker.SpeakComplete();
                    // �� �̻� ������ ��ȭ�� ���ٸ� ��ȭ ����
                    else
                    {
                        StartCoroutine(BlendingCoroutine(BlendType.EndToStart,
                            () =>
                            {
                                gameObject.SetActive(false);

                                // ��ǲ ���󺹱�
                                GameManager.Instance.InputSystem.CameraRotatable = true;
                                GameManager.Instance.Player.Moveable = true;

                                // UI ����
                                GameManager.Instance.UISystem.HUD = true;

                                // �ΰ��� UI �ѱ�
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
        /// <summary> ���� ���� ī�޶���� �� ī�޶� /// </summary>
        StartToEnd,
        /// <summary> ���� �� ī�޶���� ���� ī�޶� /// </summary>
        EndToStart,
        /// <summary> ���� �� ī�޶���� �÷��̾� ī�޶� /// </summary>
        StartToPlayer
    }
}