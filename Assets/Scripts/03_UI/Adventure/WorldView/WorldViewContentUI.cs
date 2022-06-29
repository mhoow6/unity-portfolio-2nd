using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldViewContentUI : Display
{
    public List<StageUI> StageDisplays = new List<StageUI>();

    public void SetData(int worldIdx, Vector2 parentSize)
    {
        rectTransform.sizeDelta = parentSize;

        for (int i = 0; i < StageDisplays.Count; i++)
        {
            var display = StageDisplays[i];
            display.SetData(worldIdx, i + 1);
        }
    }
}
