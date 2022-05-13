using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Migration : MonoBehaviour
{
    public Camera Camera;
    public Light DirectionalLight;

    /// <summary> GameManager�� �������� �ű�ϴ�. </summary>
    public void Do()
    {
        // �� ������ �ִ� Environment�� ���ӸŴ����� �����鿡 �Ҵ�
        var gm = GameManager.Instance;
        if (gm)
        {
            GameManager.Instance.MainCam = Camera;
            GameManager.Instance.DirectionalLight = DirectionalLight;
        }
        // ������ �������� �ı�
        Destroy(gameObject);
    }
}