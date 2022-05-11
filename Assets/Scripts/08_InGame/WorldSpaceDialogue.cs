using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WorldSpaceDialogue : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera m_DialogueLookCamera;
    GameObject m_BeforeCamera;

    private void Start()
    {
        StartCoroutine(TestBlends());
    }

    IEnumerator TestBlends()
    {
        yield return new WaitForSeconds(3f);
        SetBlendSetting(GameManager.Instance.MainCam.GetComponent<CinemachineBrain>());
    }

    void SetBlendSetting(CinemachineBrain brain)
    {
        var path = GameManager.Instance.Config.CinemachineBlendSettingsPath;
        GameManager.Instance.InputSystem.CameraRotatable = false;

        // 사전에 세팅한 BlenderSetting을 brain에 적용시킨다.
        brain.m_CustomBlends = Resources.Load<CinemachineBlenderSettings>($"{path}/DialogueBlends");

        // From을 플레이어 카메라, To를 대화창을 바라보는 카메라로 바꾼다.
        // Style과 Time은 세팅이 이미 되었을 것이므로 패스
        brain.m_CustomBlends.m_CustomBlends[0].m_From = GameManager.Instance.FreeLookCam.name;
        brain.m_CustomBlends.m_CustomBlends[0].m_To = m_DialogueLookCamera.name;

        // 대화창을 끝내고 돌아가기 위해 플레이어 카메라를 before에 넣어주고 비활성화
        m_BeforeCamera = GameManager.Instance.FreeLookCam.gameObject;
        m_BeforeCamera.SetActive(false);

        // 대화창을 바라보는 카메라를 활성화시켜 블랜딩
        m_DialogueLookCamera.gameObject.SetActive(true);
    }
}
