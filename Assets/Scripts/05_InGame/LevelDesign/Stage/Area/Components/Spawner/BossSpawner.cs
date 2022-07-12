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
        Vector2 desired = new Vector2(-1.5f, 0);

        while (timer < 2f)
        {
            timer += Time.deltaTime;

            GameManager.InputSystem.CameraRotateInput = Vector2.Lerp(GameManager.InputSystem.CameraRotateInput, desired, timer / 2f);

            yield return null;
        }

        StageManager.Instance.StageClear();
    }
}
