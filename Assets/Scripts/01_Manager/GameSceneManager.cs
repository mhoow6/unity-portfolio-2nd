using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : SingleTon<GameSceneManager>
{
    public void LoadScene(string sceneName)
    {
        var texts = FileHelper.GetLinesFromTableTextAsset($"99_Table/{sceneName}");
        foreach (var text in texts)
        {
            Debug.Log($"{text}");
        }
    }
}
