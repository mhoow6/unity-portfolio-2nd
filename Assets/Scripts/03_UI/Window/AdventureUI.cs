using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureUI : UI
{
    public StatusUI StatusDisplay;
    public List<StageUI> StageDisplays = new List<StageUI>();

    public override UIType Type => UIType.Adventure;

    // UNDONE: 추후 또 다른 월드가 나오면 로직을 수정해야함
    const int WORLD_IDX = 1;

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        StatusDisplay.SetData();
        for (int i = 0; i < StageDisplays.Count; i++)
        {
            var display = StageDisplays[i];
            display.SetData(WORLD_IDX, i + 1);
        }
        
    }
}
