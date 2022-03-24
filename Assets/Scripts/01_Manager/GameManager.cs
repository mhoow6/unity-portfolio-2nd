using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);

        // Manager Load
        TableManager.Instance.LoadTable();
        // ---
    }
}
