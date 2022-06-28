using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;

#region PlayableDirector Summary
/*
 * 1. PlayableDirector
 * Timeline 인스턴스와 TimelineAsset 사이의 링크를 저장하는 클래스
 * ex. "GameObject" 라는 게임오브젝트에 PlayableDirector를 추가하여 "GameObjectTimeline" 타임라인 에셋을 PlayableDirector.Playable에 연결
 * 
 * 2. Bindings
 * Binding 영역을 사용하여 씬의 게임 오브젝트를 타임라인 에셋과 연결할 수 있다.
 * Binding 영역은 Timeline 창에서 트랙을 의미함.
 * Binding 영역은 두 가지 열로 구분되는데
 * 첫번째(앞부분)열은 타임라인의 에셋 트랙. 아이콘과 트랙타입이 나타나는 부분
 * 두번째(뒷부분)열은 트랙에 연결된 게임오브젝트
 * 
 * 3. struct PlayableBinding
 * PlayableAsset의 출력에 대한 정보를 저장하는 구조체
 * PlayableAsset은 PlayableBinding들을 사용하여 출력 유형을 지정한다.
 * 
 */
#endregion

[RequireComponent(typeof(PlayableDirector))]
public abstract class Cutscene : MonoBehaviour
{
    public bool CutSceneStart { get; private set; }
    public bool CutScenePlaying { get; private set; }
    public bool CutSceneEnd { get; private set; }

    protected PlayableDirector director;

    /// <summary>
    /// 컨트롤 트랙의 프리팹들이 보관되어있는 자료구조.<br/>
    /// 다른 트랙들에서 오브젝트가 바인딩 되고 나서 가져온다.<br/>
    /// </summary>
    protected Dictionary<string, List<GameObject>> controlTrackPrefabs = new Dictionary<string, List<GameObject>>();

    SignalReceiver _signalReceiver;
    
    protected void Awake()
    {
        director = GetComponent<PlayableDirector>();
        _signalReceiver = GetComponent<SignalReceiver>();

        // 바인딩 정보 초기화
        var timelineAsset = director.playableAsset;
        foreach (var output in timelineAsset.outputs)
            director.ClearGenericBinding(output.sourceObject);

        // 컷신 마무리시 이벤트
        director.stopped += (director) =>
        {
            CutSceneStart = true;
            CutScenePlaying = false;
            CutSceneEnd = true;

            OnCutSceneFinish();

            StopAllCoroutines();

            if (reUsable)
            {
                CutSceneStart = false;
                CutScenePlaying = false;
                CutSceneEnd = false;

                if (gameObject.activeSelf)
                    StartCoroutine(WaitingForCutsceneInput());
            }
            else
            {
                gameObject.SetActive(false);
            }
        };

        OnAwake();

        // 컷신 대기중
        StartCoroutine(WaitingForCutsceneInput());
    }

    IEnumerator WaitingForCutsceneInput()
    {
        while (true)
        {
            if (CutSceneInput() && !CutSceneStart)
            {
                CutSceneStart = true;
                CutScenePlaying = true;
                CutSceneEnd = false;

                var timelineAsset = director.playableAsset;
                foreach (var output in timelineAsset.outputs)
                {
                    Binding(output);
                    GetControlTrackPrefabs(output);

                    switch (output.streamName)
                    {
                        case "Cinemachine Track":
                            director.SetGenericBinding(output.sourceObject, cinemachineBrain);
                            break;
                    }
                }

                OnCutSceneStart();
                director.Play();

                yield break;
            }
            yield return null;
        }
    }

    void Binding(PlayableBinding playableBinding)
    {
        if (bindingKeyValuePairs == null)
            return;

        if (bindingKeyValuePairs.TryGetValue(playableBinding.streamName, out var obj))
        {
            director.SetGenericBinding(playableBinding.sourceObject, obj.BindingObject);
            obj.BindingTrackCallback?.Invoke(playableBinding);
        }
            
    }

    void GetControlTrackPrefabs(PlayableBinding playableBinding)
    {
        if (!(playableBinding.sourceObject is ControlTrack))
            return;

        if (controlTrackPrefabs.Count > 0)
            return;

        ControlTrack controlTrack = (ControlTrack)playableBinding.sourceObject;
        List<GameObject> prefabs = new List<GameObject>();
        controlTrackPrefabs.Add(playableBinding.streamName, prefabs);
        foreach (TimelineClip clip in controlTrack.GetClips())
        {
            ControlPlayableAsset playableClip = (ControlPlayableAsset)clip.asset;
            prefabs.Add(playableClip.prefabGameObject);
        }
    }

    protected abstract CinemachineBrain cinemachineBrain { get; }

    /// <summary> Key: 트랙이름 Value: 트랙에 바인딩할 오브젝트 이름 </summary>
    protected abstract Dictionary<string, BindingData> bindingKeyValuePairs { get; }

    /// <summary> 컷신 재사용 여부 </summary>
    protected abstract bool reUsable { get; }
    protected abstract bool CutSceneInput();

    /// <summary> 컷신 시작 바로 전 호출 </summary>
    protected virtual void OnCutSceneStart() { }

    /// <summary> 컷신 끝난 후 바로 호출 </summary>
    protected virtual void OnCutSceneFinish() { }
    protected virtual void OnAwake() { }
}

public struct BindingData
{
    public UnityEngine.Object BindingObject;
    public Action<PlayableBinding> BindingTrackCallback;
}