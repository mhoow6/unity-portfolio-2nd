using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [ReadOnly] public Character CurrentCharacter;
    protected List<Character> m_CurrentCharacters = new List<Character>();

    void Start()
    {
        GameManager.Instance.Player = this;

        // ���� ù��°�� Ȱ��ȭ�� ���� ���� ĳ������
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Character>();

            if (child.gameObject.activeSelf)
                CurrentCharacter = cha;
            else
                child.gameObject.SetActive(false);

            m_CurrentCharacters.Add(cha);
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }
}
