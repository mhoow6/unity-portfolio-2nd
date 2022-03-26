using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;

public class GameManager : MonoBehaviour
{
    public Configuration Config { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Config = Resources.Load<Configuration>("Configuration");

        // Manager Load
        TableManager.Instance.LoadTable();
        // ---

    }
}
