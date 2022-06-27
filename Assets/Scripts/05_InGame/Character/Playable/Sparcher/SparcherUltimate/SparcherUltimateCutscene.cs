using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#region ��� ����
/*
 * 1. ������
 * 1�ܰ� �ڽĵ��� ���� ��Ȱ��ȭ ���·� ���д�.
 */
#endregion

public class SparcherUltimateCutscene : Cutscene
{
    protected override CinemachineBrain _cinemachineBrain => StageManager.Instance.BrainCam;

    protected override Dictionary<string, UnityEngine.Object> _bindingKeyValuePairs => m_BindingKeyValuePairs;

    protected override bool _reUsable => false;

    Dictionary<string, UnityEngine.Object> m_BindingKeyValuePairs = new Dictionary<string, UnityEngine.Object>();

    [SerializeField] BoxCollider m_HitBox;

    public void Signal_HitboxOn()
    {
        m_HitBox.gameObject.SetActive(true);
        m_HitBox.enabled = true;
    }

    public void Signal_HitboxOff()
    {
        m_HitBox.enabled = false;
    }

    protected override bool CutSceneInput()
    {
        if (GameManager.InputSystem.PressXButton)
            return true;
        return false;
    }

    protected override void OnAwake()
    {
        // ���ε��� Ʈ���� ������Ʈ �����ϱ�
        var sm = StageManager.Instance;
        var character = sm.Player.CurrentCharacter; 

        m_BindingKeyValuePairs.Add("Sparcher Animation Track", character);
        m_BindingKeyValuePairs.Add("Sparcher Transform Track", character);
        m_BindingKeyValuePairs.Add("Signal Track", GetComponent<SignalReceiver>());
    }

    protected override void OnCutSceneStart()
    {
        // �ƽ� ������ҵ� Ȱ��ȭ �����༭ �۵��ϰ� �ϱ�
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            // ����
            if (child.Equals(m_HitBox.transform))
                continue;

            child.gameObject.SetActive(true);
        }
    }

    protected override void OnCutSceneFinish()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
    }
}
