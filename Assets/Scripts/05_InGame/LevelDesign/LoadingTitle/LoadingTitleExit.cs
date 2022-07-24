using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTitleExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        LobbyManager.Instance.MovingRoad.LastRoad.Attach(other.gameObject);
    }
}
