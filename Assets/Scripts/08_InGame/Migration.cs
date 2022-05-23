using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Migration : MonoBehaviour
{
    public Camera Camera;
    public Light DirectionalLight;
    public SceneType CurrentSceneType;

    /// <summary> GameManager�� �������� �ű�ϴ�. </summary>
    public void Do()
    {
        // �� ������ �ִ� Environment�� ���ӸŴ����� �����鿡 �Ҵ�
        // UNDONE: �����丵 ���
        var gm = GameManager.Instance;
        if (gm)
        {
            GameManager.Instance.MainCam = Camera;
            GameManager.Instance.DirectionalLight = DirectionalLight;
            GameManager.SceneType = CurrentSceneType;
        }
        // ������ �������� �ı�
        Destroy(gameObject);
    }
}
