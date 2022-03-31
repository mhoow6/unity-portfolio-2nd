using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : UI
{
    public Text NickName;
    public Text Level;
    public Slider ExperienceSlider;

    public override UIType Type => UIType.MainMenu;

    public void OnDataSaveBtnClick()
    {
        var gm = GameManager.Instance;
        gm.PlayerData.Delete();
    }

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        var playerData = GameManager.Instance.PlayerData;

        Debug.Log(playerData.Level);
        Debug.Log(playerData.Experience);
        Debug.Log(playerData.NickName);
    }
}
