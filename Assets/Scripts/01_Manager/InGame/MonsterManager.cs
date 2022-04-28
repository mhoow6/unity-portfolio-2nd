using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoSingleton<MonsterManager>
{
    public List<Character> Monsters = new List<Character>();
}
