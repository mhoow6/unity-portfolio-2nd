using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
    public bool cutSceneStart { get; private set; }
    public bool cutScenePlaying { get; private set; }
    public bool cutSceneEnd { get; private set; }

    protected PlayableDirector _director;
    protected SignalReceiver _signalReceiver;
    
    protected void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        _signalReceiver = GetComponent<SignalReceiver>();

        // 바인딩 정보 초기화
        var timelineAsset = _director.playableAsset;
        foreach (var output in timelineAsset.outputs)
            _director.ClearGenericBinding(output.sourceObject);

        // 컷신 마무리시 이벤트
        _director.stopped += (director) =>
        {
            cutSceneStart = true;
            cutScenePlaying = false;
            cutSceneEnd = true;

            OnCutSceneFinish();

            StopAllCoroutines();

            if (_reUsable)
            {
                cutSceneStart = false;
                cutScenePlaying = false;
                cutSceneEnd = false;

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
            if (CutSceneInput() && !cutSceneStart)
            {
                cutSceneStart = true;
                cutScenePlaying = true;
                cutSceneEnd = false;

                var timelineAsset = _director.playableAsset;
                foreach (var output in timelineAsset.outputs)
                {
                    Binding(output);

                    switch (output.streamName)
                    {
                        case "Cinemachine Track":
                            _director.SetGenericBinding(output.sourceObject, _cinemachineBrain);
                            break;
                    }
                }

                OnCutSceneStart();
                _director.Play();

                yield break;
            }
            yield return null;
        }
    }

    void Binding(PlayableBinding playableBinding)
    {
        if (_bindingKeyValuePairs.TryGetValue(playableBinding.streamName, out var obj))
            _director.SetGenericBinding(playableBinding.sourceObject, obj);
    }

    protected abstract CinemachineBrain _cinemachineBrain { get; }

    /// <summary> Key: 트랙이름 Value: 트랙에 바인딩할 오브젝트 이름 </summary>
    protected abstract Dictionary<string, UnityEngine.Object> _bindingKeyValuePairs { get; }

    /// <summary> 컷신 재사용 여부 </summary>
    protected abstract bool _reUsable { get; }
    protected abstract bool CutSceneInput();

    protected virtual void OnCutSceneStart() { }
    protected virtual void OnCutSceneFinish() { }
    protected virtual void OnAwake() { }
}
