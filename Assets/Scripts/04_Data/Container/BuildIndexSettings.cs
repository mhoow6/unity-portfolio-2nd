using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "BuildIndexSettings", order = 1)]
public sealed class BuildIndexSettings : ScriptableObject
{
    [Header("# Ű: Scene In Build�� �ε���, ��: ����� �������� �ε���")]
    public List<SceneLoadData> Pair;
}
