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

        // ������ ������ BlenderSetting�� brain�� �����Ų��.
        brain.m_CustomBlends = Resources.Load<CinemachineBlenderSettings>($"{path}/DialogueBlends");

        // From�� �÷��̾� ī�޶�, To�� ��ȭâ�� �ٶ󺸴� ī�޶�� �ٲ۴�.
        // Style�� Time�� ������ �̹� �Ǿ��� ���̹Ƿ� �н�
        brain.m_CustomBlends.m_CustomBlends[0].m_From = GameManager.Instance.FreeLookCam.name;
        brain.m_CustomBlends.m_CustomBlends[0].m_To = m_DialogueLookCamera.name;

        // ��ȭâ�� ������ ���ư��� ���� �÷��̾� ī�޶� before�� �־��ְ� ��Ȱ��ȭ
        m_BeforeCamera = GameManager.Instance.FreeLookCam.gameObject;
        m_BeforeCamera.SetActive(false);

        // ��ȭâ�� �ٶ󺸴� ī�޶� Ȱ��ȭ���� ����
        m_DialogueLookCamera.gameObject.SetActive(true);
    }
}
