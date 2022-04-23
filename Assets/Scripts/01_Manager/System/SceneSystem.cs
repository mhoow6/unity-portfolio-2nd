using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SceneSystem : GameSystem
{
    public SceneType CurrentScene { get; private set; }

    public void Init()
    {
        if (GameManager.Instance.IsTestZone)
            CurrentScene = SceneType.Test;
        else
            CurrentScene = SceneType.MainMenu;
    }

    public void Tick()
    {
        
    }

    public enum SceneType
    {
        MainMenu,
        InGame,
        Test = -1
    }
}
