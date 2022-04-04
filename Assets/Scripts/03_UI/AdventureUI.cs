using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureUI : UI
{
    public StatusDisplay StatusDisplay;
    public List<StageDisplay> StageDisplays = new List<StageDisplay>();

    public override UIType Type => UIType.Adventure;

    // ���� �� �ٸ� ���尡 ������ ������ �����ؾ���
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
