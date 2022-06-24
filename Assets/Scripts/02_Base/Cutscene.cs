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
            gameObject.SetActive(false);
        };

        // �ƽ� �����
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

    /// <summary> Key: Ʈ���̸� Value: Ʈ���� ���ε��� ������Ʈ �̸� </summary>
    protected abstract Dictionary<string, GameObject> BindingKeyValuePairs { get; }

    protected virtual void OnCutSceneStart() { }
    protected virtual void OnCutSceneFinish() { }
    protected virtual void OnAwake() { }
}
