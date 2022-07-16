using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearDetailUI : Display
{
    public Text Right;

    public void SetData(string rightText)
    {
        Right.text = rightText;
    }
}
