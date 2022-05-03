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
    public FixedQueue<AniType> AnimationQueue { get; private set; } = new FixedQueue<AniType>(1);
    public Vector3 MoveVector { get; private set; }

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
        // 여기에서 프레임별로 애니메이션을 하나씩 받아오도록 처리
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
                    AnimationQueue.Enqueue(AniType.RUN_0);
                }
                else
                {
                    // 애니메이션
                    AnimationQueue.Enqueue(AniType.IDLE_0);
                }

                // 이동
                CurrentCharacter.transform.position += moveVector * Time.deltaTime * CurrentCharacter.Data.Speed;

                MoveVector = moveVector;
            }

            yield return null;
        }
    }
}
