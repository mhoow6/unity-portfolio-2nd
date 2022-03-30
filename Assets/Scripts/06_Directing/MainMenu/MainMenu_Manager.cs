using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Manager : MonoSingleton<MainMenu_Manager>
{
    public Transform CameraPosition;

    const float CAMERA_MOVE_SPEED = 0.5F;

    public void StartDirecting()
    {
        StartCoroutine(MakeCameraMovingToPosition());
    }

    IEnumerator MakeCameraMovingToPosition()
    {
        float timer = 0f;
        float sensitivity = 0f;
        Camera mainCam = GameManager.Instance.MainCam;
        Vector3 goalPosition = CameraPosition.transform.position;

        while (timer < 4f)
        {
            timer += Time.deltaTime;
            sensitivity += timer * 0.001f;
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, goalPosition, sensitivity * CAMERA_MOVE_SPEED);
            yield return null;
        }
        GameManager.Instance.UISystem.OpenWindow(UIType.MainMenu);
    }
}
