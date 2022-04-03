using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Character CurrentCharacter { get; protected set; }
    protected Character[] m_CurrentCharacters;

    protected virtual void Start()
    {
        GameManager.Instance.Player = this;

        // 제일 첫번째로 활성화된 것이 현재 캐릭터임
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
                CurrentCharacter = child.GetComponent<Character>();
            else
                child.gameObject.SetActive(false);
        }
    }

    protected virtual void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }
}
