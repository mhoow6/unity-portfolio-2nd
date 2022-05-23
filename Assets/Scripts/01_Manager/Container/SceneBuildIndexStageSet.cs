using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "SceneIndexToStage", order = 1)]
public sealed class SceneBuildIndexStageSet : ScriptableObject
{
    [Header("# Ű: Scene In Build�� �ε���, ��: ����� �������� �ε���")]
    public List<SceneBuildStage> Pair;
}
