using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGlobalData", menuName = "GameGlobalData", order = 1)]
public class GameGlobalData : ScriptableObject
{
    public List<Color> RarityBackgroundColors => m_RarityBackgroundColors;
    [SerializeField] List<Color> m_RarityBackgroundColors = new List<Color>(5);
}
