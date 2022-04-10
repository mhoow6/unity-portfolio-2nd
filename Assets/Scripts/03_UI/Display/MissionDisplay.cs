using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionDisplay : Display
{
    public GameObject ClearMedal;
    public GameObject ClearText;
    public Text MissionText;
    
    public void SetData(string missionText, bool clear)
    {
        ClearMedal.gameObject.SetActive(clear);
        ClearText.gameObject.SetActive(clear);

        MissionText.text = missionText;
    }
}
