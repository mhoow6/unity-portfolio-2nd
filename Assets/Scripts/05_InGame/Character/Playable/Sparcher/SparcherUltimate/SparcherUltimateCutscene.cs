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
        // ���ε��� Ʈ���� ������Ʈ �����ϱ�
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

        var skillData = Character.GetSkillData(Character.GetUltimateIndex(ObjectCode.CHAR_Sparcher));
        if (skillData)
        {
            var sparcherUltiData = skillData as DatabaseSystem.SparcherUltiData;
            float range = sparcherUltiData.HitBoxRange;
            m_HitBox.size = new Vector3(range, 1, range);
        }
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

        // ĳ���� �� �����̰� �ϱ�
        GameManager.InputSystem.CameraRotatable = false;
        StageManager.Instance.Player.Moveable = false;
        var inGameUi = GameManager.UISystem.CurrentWindow as InGameUI;
        inGameUi.Joystick.gameObject.SetActive(false);
    }

    protected override void OnCutSceneFinish()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }

        var timelineAsset = director.playableAsset;
        foreach (var output in timelineAsset.outputs)
        {
            // ��ġ�� �����ߴ��� �ʱ�ȭ ��Ű��
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

        // ĳ���� �� �����̰� �ϱ�
        GameManager.InputSystem.CameraRotatable = true;
        StageManager.Instance.Player.Moveable = true;
        var inGameUi = GameManager.UISystem.CurrentWindow as InGameUI;

        if (!inGameUi.Joystick.IsNullOrDestroyed())
            inGameUi.Joystick.gameObject.SetActive(true);
    }
}
