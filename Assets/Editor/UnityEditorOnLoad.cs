using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class UnityEditorOnLoad
{
    static UnityEditorOnLoad()
    {
        Debug.LogWarning($"UnityEditor On Load");

        PlayerSettings.Android.keystorePass = "123456";
        PlayerSettings.Android.keyaliasPass = "123456";
    }
}
