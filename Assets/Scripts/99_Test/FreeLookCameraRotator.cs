using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookCameraRotator : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook CinemachineFreeLook;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float value = -1 * (timer % 2);

        CinemachineFreeLook.m_XAxis.Value = value;
    }
}
