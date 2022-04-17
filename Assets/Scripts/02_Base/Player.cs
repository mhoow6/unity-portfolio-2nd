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

        // ���� ù��°�� Ȱ��ȭ�� ���� ���� ĳ������
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

    private void Update()
    {
        // �ӽ�
        if (Input.GetKeyDown(KeyCode.A))
            GameManager.Instance.UISystem.OpenWindow(UIType.InGame);

        // ĳ���� �����̱�
        var inputAxis = GameManager.Instance.InputSystem.InputAxis;
        if (inputAxis != null)
        {
            // �̵�
            
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }
}
