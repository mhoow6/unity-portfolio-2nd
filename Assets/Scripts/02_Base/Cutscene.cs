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
    public bool CutSceneStart { get; private set; }
    public bool CutScenePlaying { get; private set; }
    public bool CutSceneEnd { get; private set; }

    protected PlayableDirector director;

    /// <summary>
    /// ��Ʈ�� Ʈ���� �����յ��� �����Ǿ��ִ� �ڷᱸ��.<br/>
    /// �ٸ� Ʈ���鿡�� ������Ʈ�� ���ε� �ǰ� ���� �����´�.<br/>
    /// </summary>
    protected Dictionary<string, List<GameObject>> controlTrackPrefabs = new Dictionary<string, List<GameObject>>();

    SignalReceiver _signalReceiver;
    
    protected void Awake()
    {
        director = GetComponent<PlayableDirector>();
        _signalReceiver = GetComponent<SignalReceiver>();

        // ���ε� ���� �ʱ�ȭ
        var timelineAsset = director.playableAsset;
        foreach (var output in timelineAsset.outputs)
            director.ClearGenericBinding(output.sourceObject);

        // �ƽ� �������� �̺�Ʈ
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

        // �ƽ� �����
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

    /// <summary> Key: Ʈ���̸� Value: Ʈ���� ���ε��� ������Ʈ �̸� </summary>
    protected abstract Dictionary<string, BindingData> bindingKeyValuePairs { get; }

    /// <summary> �ƽ� ���� ���� </summary>
    protected abstract bool reUsable { get; }
    protected abstract bool CutSceneInput();

    /// <summary> �ƽ� ���� �ٷ� �� ȣ�� </summary>
    protected virtual void OnCutSceneStart() { }

    /// <summary> �ƽ� ���� �� �ٷ� ȣ�� </summary>
    protected virtual void OnCutSceneFinish() { }
    protected virtual void OnAwake() { }
}

public struct BindingData
{
    public UnityEngine.Object BindingObject;
    public Action<PlayableBinding> BindingTrackCallback;
}