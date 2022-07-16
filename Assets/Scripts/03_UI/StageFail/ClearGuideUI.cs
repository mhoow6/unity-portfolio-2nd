using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearGuideUI : Display
{
    public Text GuideText;

    public void SetData(string guideText)
    {
        GuideText.text = guideText;
    }
}
