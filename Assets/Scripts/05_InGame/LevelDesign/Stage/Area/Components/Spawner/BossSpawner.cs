using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : AreaSpawner, IStageClearable
{
    [Header("# ���� ������ ��������"), Tooltip("������ �� óġ�ϸ� ���������� Ŭ���� �˴ϴ�.")]
    public bool KillBossToStageClear;

    public void ClearAction()
    {
        if (KillBossToStageClear)
        {
            // ������ ī�޶� ���� OFF
            GameManager.InputSystem.CameraRotatable = false;

            // ���� ī�޶� ��ġ���� �÷��̾ �������� ȸ�� (EaseIn)
            StartCoroutine(StageClearCoroutine());
        }
    }

    IEnumerator StageClearCoroutine()
    {
        float timer = 0f;
        var freelook = StageManager.Instance.FreeLookCam;

        while (timer < 2f)
        {
            timer += Time.deltaTime;

            freelook.m_XAxis.Value = Mathf.Lerp(freelook.m_XAxis.Value, -2.0f, timer / 2f);

            yield return null;
        }

        StageManager.Instance.StageClear();
    }
}
