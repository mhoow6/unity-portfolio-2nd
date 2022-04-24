using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
    [ReadOnly] public Character CurrentCharacter;
    [ReadOnly] public List<Character> Characters = new List<Character>();
    public bool Controlable
    {
        set
        {
            if (value)
                StartCoroutine(m_ControlCoroutine);
            else
                StopCoroutine(m_ControlCoroutine);
        }
    }
    public Queue<AniType> AnimationQueue { get; private set; } = new Queue<AniType>();

    IEnumerator m_ControlCoroutine;

    const float ROTATE_SENSTIVITY = 6f;

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

        m_ControlCoroutine = ControlCoroutine();
    }

    private void Update()
    {
        // ���⿡�� �����Ӻ��� �ִϸ��̼��� �ϳ��� �޾ƿ����� ó��
        if (CurrentCharacter != null && AnimationQueue.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AnimationQueue.Dequeue());
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }

    IEnumerator ControlCoroutine()
    {
        while (true)
        {
            // ĳ���� �����̱�
            var ControllerInput = GameManager.Instance.InputSystem.ControllerInput;
            var cam = GameManager.Instance.MainCam;
            if (cam != null)
            {
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * ControllerInput.y + cameraRight * ControllerInput.x;

                if (moveVector.magnitude != 0)
                {
                    // ȸ��
                    CurrentCharacter.transform.forward = Vector3.Lerp(CurrentCharacter.transform.forward, moveVector.normalized, Time.deltaTime * ROTATE_SENSTIVITY);

                    // �ִϸ��̼�
                    AnimationQueue.Enqueue(AniType.RUN_0);
                }
                else
                {
                    // �ִϸ��̼�
                    AnimationQueue.Enqueue(AniType.IDLE_0);
                }

                // �̵�
                CurrentCharacter.transform.position += moveVector * Time.deltaTime * CurrentCharacter.Data.Speed;
            }

            yield return null;
        }
    }
}
