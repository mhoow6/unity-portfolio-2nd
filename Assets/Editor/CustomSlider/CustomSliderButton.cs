using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomSlider))]
public class CustomSliderButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CustomSlider generator = (CustomSlider)target;

        if (GUILayout.Button("Create Elements", new GUILayoutOption[] { GUILayout.Height(30) }))
            generator.CreateElements();

        if (GUILayout.Button("Delete Elements", new GUILayoutOption[] { GUILayout.Height(30) }))
            generator.DestroyElements();
    }
}
