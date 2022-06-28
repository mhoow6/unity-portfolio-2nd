using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DatabaseSystem;
using System;
using DG.Tweening;

#region ��� ����
/*
 * 1. ������
 * WorldSpaceDialogue ��Ʈ�� 1�ܰ� �ڽĵ��� ���� ��Ȱ��ȭ ���·� ���д�.
 * WorldSpaceDialogue�� Ȱ��ȭ���������� 1�ܰ� �ڽĵ��� Ȱ��ȭ���� �ʰ� �ϱ� ����.
 */
#endregion

public class WorldSpaceDialogue : MonoBehaviour
{
    [SerializeField] CinemachineBlenderSettings m_BlenderSettings;
    [SerializeField] CinemachineVirtualCamera m_StartBlendingCamera;
    [SerializeField] CinemachineVirtualCamera m_EndBlendingCamera;
    [SerializeField] GameObject m_EndBlendingPostProcessing;

    // �÷��̾� ���� ������Ű�� �뵵
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

    /// <summary> � ��ȭ�ڰ� ���ϰ� �ִ°�?</summary>
    SpeakerPreset m_CurrentSpeaker;
    [SerializeField] Text m_DialogueText;

    const float DIALOUGE_WINDOW_TWEENING_SPEED = 0.3f;

    public void SetData(List<StageDialogueTable> dialogues)
    {
        // ��ȭ��� ����
        m_Dialogues = dialogues;

        // Blend ����
        SetBlendSetting();

        // ĵ���� ī�޶� ����
        m_Canvas.worldCamera = StageManager.Instance.MainCam;

        // UI ����
        GameManager.UISystem.HUD = false;

        // ī�޶� ����
        var brain = StageManager.Instance.BrainCam;
        StartCoroutine(BlendingCoroutine(BlendType.StartToEnd, () => 
        {
            Invoke("DialogueSetting", 0.5f);
        }));      
    }

    // Canvas-Button-OnClickEvent
    public void DialogueRead()
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
                m_Monitor.transform.DOScaleY(0, DIALOUGE_WINDOW_TWEENING_SPEED);

                StartCoroutine(BlendingCoroutine(BlendType.EndToStart,
                    () =>
                    {
                        gameObject.SetActive(false);

                        GameManager.InputSystem.CameraRotatable = true;
                        StageManager.Instance.Player.Moveable = true;

                        // UI �ѱ�
                        GameManager.UISystem.HUD = true;

                        // �ΰ��� UI �ѱ�
                        GameManager.UISystem.OpenWindow(UIType.InGame);
                    }));
            }
        }
    }

    void SetBlendSetting()
    {
        var sm = StageManager.Instance;
        var brain = sm.BrainCam;

        // ���� ������ ���� ī�޶�� 
        GameManager.InputSystem.CameraRotatable = false;
        // �����Ҷ� ī�޶� ��ġ �����ϱ�
        sm.FreeLookCam.transform.SetPositionAndRotation(m_StartBlendingCamera.transform.position, m_StartBlendingCamera.transform.rotation);

        // ĳ���� ������ ����
        sm.Player.Moveable = false;
        // ĳ���� ����ġ
        sm.Player.CurrentCharacter.transform.SetPositionAndRotation(m_CharacterFixedTransform.position, m_CharacterFixedTransform.rotation);
        sm.Player.CurrentCharacter.TryAttachToFloor();

        // startBlendingCamera ��ġ�� ���� ī�޶� ��ġ�� �����ϰ� �ϱ�
        //m_StartBlendingCamera.transform.position = sm.FreeLookCam.transform.position;
        //m_StartBlendingCamera.transform.rotation = sm.FreeLookCam.transform.rotation;

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
        brain.m_CustomBlends.m_CustomBlends[(int)BlendType.StartToPlayer].m_To = sm.FreeLookCam.name;
    }

    IEnumerator BlendingCoroutine(BlendType type, Action blendDoneCallback = null)
    {
        var brain = StageManager.Instance.BrainCam;
        switch (type)
        {
            case BlendType.StartToEnd:
                // ��ȭâ ���� �� ���� ī�޶�� ���ư����Ѵ�
                m_ReturnCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject;

                // ���� ī�޶� ����
                brain.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
                // ���� ���� ī�޶� �ѱ�
                m_StartBlendingCamera.gameObject.SetActive(true);

                // ���� ��ٸ� ��,
                yield return new WaitForSeconds(1f);

                // ���� ī�޶� ��Ȱ��ȭ �ϰ�
                m_StartBlendingCamera.gameObject.SetActive(false);

                // ��ȭâ�� �ٶ󺸴� ī�޶� Ȱ��ȭ���� ����
                m_EndBlendingCamera.gameObject.SetActive(true);
                // ����Ʈ ���μ��̵� ���� ���ش�.
                m_EndBlendingPostProcessing.SetActive(true);
                break;
            case BlendType.EndToStart:
                // ��ȭâ�� �ٶ󺸴� ī�޶� ��Ȱ��ȭ�ϰ�
                m_EndBlendingCamera.gameObject.SetActive(false);
                // ����Ʈ ���μ��̵� ���� ���ְ�
                m_EndBlendingPostProcessing.SetActive(false);

                // ���� ���� ī�޶� �Ѽ� ����
                m_StartBlendingCamera.gameObject.SetActive(true);

                // ������ ��ٸ� ��,
                yield return new WaitUntil(() => !brain.IsBlending);

                // ���� ���� ī�޶� �� �ڿ�
                m_StartBlendingCamera.gameObject.SetActive(false);
                // �ٽ� ���� ī�޶� Ű��
                m_ReturnCamera.SetActive(true);

                m_ReturnCamera = null;
                yield return null;
                break;
            default:
                break;
        }

        // Ȱ��ȭ�� ī�޶� ����ġ�� �ö����� ��ٸ���
        yield return new WaitUntil(() => !brain.IsBlending);
        blendDoneCallback?.Invoke();
    }

    // ù ��ȭ�� ����
    // SetData���� Invoke���� ȣ��
    void DialogueSetting()
    {
        // ��ȭâ Ű��
        m_Monitor.gameObject.SetActive(true);

        // ��ȭâ Ʈ����
        float desired = m_Monitor.transform.localScale.y;
        m_Monitor.transform.localScale = new Vector3(m_Monitor.transform.localScale.x, 0, m_Monitor.transform.localScale.z);
        m_Monitor.transform.DOScaleY(desired, DIALOUGE_WINDOW_TWEENING_SPEED);

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