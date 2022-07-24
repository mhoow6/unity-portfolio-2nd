using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageManager))]
public class StageManagerButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        StageManager generator = (StageManager)target;

        if (GUILayout.Button("Stage Clear", new GUILayoutOption[] { GUILayout.Height(30) }))
            generator.StageClear();

        if (GUILayout.Button("Stage Fail", new GUILayoutOption[] { GUILayout.Height(30) }))
            generator.StageFail();
    }
}
