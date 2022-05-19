using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "SceneIndexToStage", order = 1)]
public sealed class SceneBuildIndexStageSetPair : ScriptableObject
{
    [Header("# 키: Scene In Build의 인덱스, 값: 월드와 스테이지 인덱스")]
    public List<SceneBuildStage> SceneBuildStages;
}
