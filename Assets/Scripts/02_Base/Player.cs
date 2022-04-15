using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [ReadOnly] public Character CurrentCharacter;
    [ReadOnly] public List<Character> Characters = new List<Character>();

    void Start()
    {
        GameManager.Instance.Player = this;

        // 제일 첫번째로 활성화된 것이 현재 캐릭터임
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Character>();

            if (child.gameObject.activeSelf)
                CurrentCharacter = cha;
            else
                child.gameObject.SetActive(false);

            Characters.Add(cha);
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }
}
