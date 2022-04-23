using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : MonoBehaviour
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
        // ĳ���� �����̱�
        var inputAxis = GameManager.Instance.InputSystem.Controller;
        var cam = GameManager.Instance.MainCam;
        if (inputAxis != null && cam != null)
        {
            Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
            Vector3 moveVector = cameraForward * inputAxis.Input.y + cameraRight * inputAxis.Input.x;

            // ȸ��
            if (moveVector.magnitude != 0)
                CurrentCharacter.transform.forward = moveVector.normalized;

            // �̵�
            CurrentCharacter.transform.position += moveVector * Time.deltaTime * CurrentCharacter.Data.Speed;
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }
}
