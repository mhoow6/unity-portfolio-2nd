using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Player : Player
{
    private void FixedUpdate()
    {
        // 왼쪽 클릭시 && 카메라가 정 위치에 있는지
        if (Input.GetMouseButtonDown(0) && MainMenu_Director.Instance.CameraInPosiiton)
        {
            RaycastHit hitInfo;
            Ray ray = GameManager.Instance.MainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                if (hitInfo.collider.gameObject.Equals(this))
                {

                }
            }
        }
    }
}
