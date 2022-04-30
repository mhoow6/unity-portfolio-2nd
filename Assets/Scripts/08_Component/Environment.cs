using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Camera Camera;
    public Light DirectionalLight;

    private void Start()
    {
        // �� ������ �ִ� Environment�� ���ӸŴ����� �����鿡 �Ҵ�
        GameManager.Instance.MainCam = Camera;
        GameManager.Instance.DirectionalLight = DirectionalLight;
    }
}
