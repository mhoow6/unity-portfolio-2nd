using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodaingTitle_CameraRotateTrigger : MonoBehaviour
{
    GameObject m_mainCamera;

    void Start()
    {
        m_mainCamera = Camera.main.gameObject;
    }

    void FixedUpdate()
    {
        if (CollideHelper.TrySphereCollide(gameObject, m_mainCamera, 1))
            m_mainCamera.transform.Rotate(Vector3.up, 180);
    }
}
