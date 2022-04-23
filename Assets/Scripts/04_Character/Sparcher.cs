using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparcher : Character
{
    public override ObjectCode Code => ObjectCode.CHAR_Sparcher;

    protected override void OnSpawn()
    {
        var currentScene = GameManager.Instance.SceneSystem.CurrentScene;
        switch (currentScene)
        {
            case SceneSystem.SceneType.MainMenu:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("09_AnimationController/Sparcher/MainMenu_Sparcher");
                break;
            case SceneSystem.SceneType.InGame:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("09_AnimationController/Sparcher/InGame_Sparcher");
                break;
            case SceneSystem.SceneType.Test:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("09_AnimationController/Sparcher/InGame_Sparcher");
                break;
        }
    }
}
