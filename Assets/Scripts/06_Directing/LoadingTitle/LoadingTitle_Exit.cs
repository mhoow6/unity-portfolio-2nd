using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTitle_Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        LoadingTitle_Manager.Instance.FarRoad.Attach(other.gameObject);
    }
}
