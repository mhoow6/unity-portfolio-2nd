using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected virtual void Start()
    {
        GameManager.Instance.Player = this;
    }

    protected virtual void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }
}
