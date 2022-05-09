using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Migration : MonoBehaviour
{
    public Camera Camera;
    public Light DirectionalLight;

    /// <summary> GameManager에 변수들을 옮깁니다. </summary>
    public void Do()
    {
        // 각 씬마다 있는 Environment가 게임매니저의 변수들에 할당
        var gm = GameManager.Instance;
        if (gm)
        {
            GameManager.Instance.MainCam = Camera;
            GameManager.Instance.DirectionalLight = DirectionalLight;
        }
        // 역할이 다했으니 파괴
        Destroy(gameObject);
    }
}
