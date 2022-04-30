using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Camera Camera;
    public Light DirectionalLight;

    private void Start()
    {
        // 각 씬마다 있는 Environment가 게임매니저의 변수들에 할당
        GameManager.Instance.MainCam = Camera;
        GameManager.Instance.DirectionalLight = DirectionalLight;
    }
}
