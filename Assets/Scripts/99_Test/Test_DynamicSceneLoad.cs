using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_DynamicSceneLoad : MonoBehaviour
{
    void Start()
    {
        GameSceneManager.Instance.LoadScene("Village");
    }
}
