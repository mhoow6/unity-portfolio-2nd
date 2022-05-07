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
    public FixedQueue<AniType> Commands { get; private set; } = new FixedQueue<AniType>(1);
    public Vector3 MoveVector { get; private set; }
    public bool Moveable;

    IEnumerator m_ControlCoroutine;

    const float m_ROTATE_SENSTIVITY = 12f;

    void Start()
    {
        GameManager.Instance.Player = this;

        // 제일 첫번째로 활성화된 것이 현재 캐릭터임
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Character>();
            cha.tag = "Player";

            if (child.gameObject.activeSelf)
                CurrentCharacter = cha;
            else
                child.gameObject.SetActive(false);

            Characters.Add(cha);
        }

        m_ControlCoroutine = ControlCoroutine();
        Moveable = true;

        // 이 게임에서 이 기능들은 쓸모없으니 끔
        CurrentCharacter.Agent.updateRotation = false;
    }

    private void Update()
    {
        // 여기에서 프레임별로 애니메이션을 하나씩 받아오도록 처리
        if (CurrentCharacter != null && Commands.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)Commands.Dequeue());
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }

    IEnumerator ControlCoroutine()
    {
        while (true)
        {
            // 캐릭터 움직이기
            var ControllerInput = GameManager.Instance.InputSystem.CharacterMoveInput;
            var cam = GameManager.Instance.MainCam;
            if (cam != null)
            {
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * ControllerInput.y + cameraRight * ControllerInput.x;

                if (moveVector.magnitude != 0)
                {
                    // 회전
                    CurrentCharacter.transform.forward = Vector3.Lerp(CurrentCharacter.transform.forward, moveVector.normalized, Time.deltaTime * m_ROTATE_SENSTIVITY);

                    // 애니메이션
                    Commands.Enqueue(AniType.RUN_0);
                }
                else
                {
                    // 애니메이션
                    Commands.Enqueue(AniType.IDLE_0);
                }

                // 이동
                if (Moveable)
                    CurrentCharacter.transform.position += moveVector * Time.deltaTime * CurrentCharacter.Speed;

                MoveVector = moveVector;
            }

            yield return null;
        }
    }
}
