using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#region 사용 설명서
/*
 * 1. 프리팹
 * 1단계 자식들은 전부 비활성화 상태로 나둔다.
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
        // 바인딩할 트랙과 오브젝트 지정하기
        var sm = StageManager.Instance;
        var character = sm.Player.CurrentCharacter; 

        m_BindingKeyValuePairs.Add("Sparcher Animation Track", character);
        m_BindingKeyValuePairs.Add("Sparcher Transform Track", character);
        m_BindingKeyValuePairs.Add("Signal Track", GetComponent<SignalReceiver>());
    }

    protected override void OnCutSceneStart()
    {
        // 컷신 구성요소들 활성화 시켜줘서 작동하게 하기
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            // 예외
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
