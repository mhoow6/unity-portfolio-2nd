using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : AreaSpawner, IStageClearable
{
    [Header("# 보스 스포너 수동기입"), Tooltip("보스를 다 처치하면 스테이지가 클리어 됩니다.")]
    public bool KillBossToStageClear;

    public void ClearAction()
    {
        if (KillBossToStageClear)
        {
            // 유저의 카메라 조작 OFF
            GameManager.InputSystem.CameraRotatable = false;

            // 현재 카메라 위치에서 플레이어를 기준으로 회전 (EaseIn)
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
