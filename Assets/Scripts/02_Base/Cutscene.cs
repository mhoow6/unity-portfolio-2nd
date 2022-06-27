using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#region PlayableDirector Summary
/*
 * 1. PlayableDirector
 * Timeline �ν��Ͻ��� TimelineAsset ������ ��ũ�� �����ϴ� Ŭ����
 * ex. "GameObject" ��� ���ӿ�����Ʈ�� PlayableDirector�� �߰��Ͽ� "GameObjectTimeline" Ÿ�Ӷ��� ������ PlayableDirector.Playable�� ����
 * 
 * 2. Bindings
 * Binding ������ ����Ͽ� ���� ���� ������Ʈ�� Ÿ�Ӷ��� ���°� ������ �� �ִ�.
 * Binding ������ Timeline â���� Ʈ���� �ǹ���.
 * Binding ������ �� ���� ���� ���еǴµ�
 * ù��°(�պκ�)���� Ÿ�Ӷ����� ���� Ʈ��. �����ܰ� Ʈ��Ÿ���� ��Ÿ���� �κ�
 * �ι�°(�޺κ�)���� Ʈ���� ����� ���ӿ�����Ʈ
 * 
 * 3. struct PlayableBinding
 * PlayableAsset�� ��¿� ���� ������ �����ϴ� ����ü
 * PlayableAsset�� PlayableBinding���� ����Ͽ� ��� ������ �����Ѵ�.
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

        // ���ε� ���� �ʱ�ȭ
        var timelineAsset = _director.playableAsset;
        foreach (var output in timelineAsset.outputs)
            _director.ClearGenericBinding(output.sourceObject);

        // �ƽ� �������� �̺�Ʈ
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

        // �ƽ� �����
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

    /// <summary> Key: Ʈ���̸� Value: Ʈ���� ���ε��� ������Ʈ �̸� </summary>
    protected abstract Dictionary<string, UnityEngine.Object> _bindingKeyValuePairs { get; }

    /// <summary> �ƽ� ���� ���� </summary>
    protected abstract bool _reUsable { get; }
    protected abstract bool CutSceneInput();

    protected virtual void OnCutSceneStart() { }
    protected virtual void OnCutSceneFinish() { }
    protected virtual void OnAwake() { }
}
