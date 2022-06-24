using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparcherUltimateCutscene : Cutscene
{
    protected override CinemachineBrain CinemachineBrain => StageManager.Instance.BrainCam;

    protected override Dictionary<string, GameObject> BindingKeyValuePairs => m_BindingKeyValuePairs;
    Dictionary<string, GameObject> m_BindingKeyValuePairs = new Dictionary<string, GameObject>();

    protected override bool CutSceneInput()
    {
        if (GameManager.InputSystem.PressXButton)
            return true;
        return false;
    }

    protected override void OnAwake()
    {
        // ���ε��� Ʈ���� ������Ʈ �����ϱ�
    }
}
