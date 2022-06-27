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
    protected override CinemachineBrain cinemachineBrain => StageManager.Instance.BrainCam;
    protected override Dictionary<string, BindingData> bindingKeyValuePairs => m_BindingKeyValuePairs;
    protected override bool reUsable => true;

    Dictionary<string, BindingData> m_BindingKeyValuePairs = new Dictionary<string, BindingData>();
    [SerializeField] BoxCollider m_HitBox;

    public void Signal_EffectOn(){ }

    public void Signal_EffectOff(){ }

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
        if (GameManager.InputSystem.PressBButton)
            return true;
        return false;
    }

    protected override void OnAwake()
    {
        // 바인딩할 트랙과 오브젝트 지정하기
        var sm = StageManager.Instance;
        var character = sm.Player.CurrentCharacter;

        m_BindingKeyValuePairs.Add("Sparcher Animation Track", new BindingData()
        {
            BindingObject = character.gameObject,
            BindingTrackCallback = (track) =>
            {
                var animeTrack = track.sourceObject as AnimationTrack;
                var secondTrackClips = animeTrack.GetClips();
                foreach (var clip in secondTrackClips)
                {
                    var animationPlayableAsset = clip.asset as AnimationPlayableAsset;
                    animationPlayableAsset.position = character.transform.position;
                    animationPlayableAsset.rotation = character.transform.rotation;
                }
            }
        });
        m_BindingKeyValuePairs.Add("Signal Track", new BindingData()
        {
            BindingObject = GetComponent<SignalReceiver>(),
        });
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

        // 위치값 조절했던거 초기화 시키기
        var timelineAsset = director.playableAsset;
        foreach (var output in timelineAsset.outputs)
        {
            if (output.sourceObject is AnimationTrack)
            {
                var animeTrack = output.sourceObject as AnimationTrack;
                var secondTrackClips = animeTrack.GetClips();
                foreach (var clip in secondTrackClips)
                {
                    var animationPlayableAsset = clip.asset as AnimationPlayableAsset;
                    animationPlayableAsset.position = Vector3.zero;
                    animationPlayableAsset.rotation = Quaternion.identity;
                }
            }
            
        }
    }
}
