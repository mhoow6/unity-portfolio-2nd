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
    public List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
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
            gameObject.SetActive(false);
        };

        // 컷신 대기중
        StartCoroutine(WaitingForCutsceneInput());

        OnAwake();
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
                            _director.SetGenericBinding(output.sourceObject, CinemachineBrain);
                            break;
                    }
                }

                _director.Play();

                OnCutSceneStart();

                yield break;
            }
        }
    }

    void Binding(PlayableBinding playableBinding)
    {
        foreach (var kvp in BindingKeyValuePairs)
        {
            string bindingStreamName = kvp.Key;
            GameObject bindingObject = kvp.Value;

            if (playableBinding.streamName.Equals(bindingStreamName))
            {
                _director.SetGenericBinding(playableBinding.sourceObject, bindingObject);
                return;
            }
        }
    }

    protected abstract CinemachineBrain CinemachineBrain { get; }
    protected abstract bool CutSceneInput();

    /// <summary> Key: 트랙이름 Value: 트랙에 바인딩할 오브젝트 이름 </summary>
    protected abstract Dictionary<string, GameObject> BindingKeyValuePairs { get; }

    protected virtual void OnCutSceneStart() { }
    protected virtual void OnCutSceneFinish() { }
    protected virtual void OnAwake() { }
}
