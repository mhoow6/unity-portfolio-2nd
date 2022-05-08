using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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
    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);
    public Vector3 MoveVector { get; private set; }
    public bool Moveable;

    [SerializeField] GameObject m_ShadowProjector;
    Vector3 m_ProjectorOffset;

    IEnumerator m_ControlCoroutine;

    const float m_ROTATE_SPEED = 20f;

    void Start()
    {
        GameManager.Instance.Player = this;

        // 제일 첫번째로 활성화된 것이 현재 캐릭터임
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Character>();
            if (cha != null)
            {
                cha.tag = "Player";

                if (child.gameObject.activeSelf)
                    CurrentCharacter = cha;
                else
                    child.gameObject.SetActive(false);

                Characters.Add(cha);
            }
        }

        m_ControlCoroutine = ControlCoroutine();
        Moveable = true;

        m_ProjectorOffset = m_ShadowProjector.transform.position - CurrentCharacter.transform.position;
    }

    private void Update()
    {
        // 여기에서 프레임별로 애니메이션을 하나씩 받아오도록 처리
        if (CurrentCharacter != null && AnimationJobs.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());

        // ShadowProjector가 캐릭터를 따라다니도록 함
        m_ShadowProjector.transform.position = CurrentCharacter.transform.position + m_ProjectorOffset;
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
                    float angle = Vector3.SignedAngle(CurrentCharacter.transform.forward, moveVector, Vector3.up);
                    Vector3 angularVelocity = new Vector3(0, angle * Time.fixedDeltaTime * m_ROTATE_SPEED, 0);
                    CurrentCharacter.Rigidbody.MoveRotation(CurrentCharacter.Rigidbody.rotation * Quaternion.Euler(angularVelocity));

                    // 애니메이션
                    AnimationJobs.Enqueue(AniType.RUN_0);
                }
                else
                {
                    // 애니메이션
                    AnimationJobs.Enqueue(AniType.IDLE_0);
                }

                // 이동
                if (Moveable)
                {
                    Vector3 desired = CurrentCharacter.transform.position + (moveVector * CurrentCharacter.Speed * Time.deltaTime);
                    CurrentCharacter.Rigidbody.MovePosition(desired);
                }

                MoveVector = moveVector;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
