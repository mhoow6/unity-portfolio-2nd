using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Player : MonoBehaviour
{
    [ReadOnly] public Playable CurrentCharacter;
    [ReadOnly] public List<Playable> Characters = new List<Playable>();
    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);

    void Update()
    {
        // 여기에서 프레임별로 애니메이션을 하나씩 받아오도록 처리
        if (AnimationJobs.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());
    }

    void OnDestroy()
    {
        ReleaseFromManager();
    }

    #region 초기화
    bool m_Init = false;

    /// <summary> 새로운 씬에서 플레이어 초기화하기 </summary>
    public void Init()
    {
        if (m_Init)
            return;
        m_Init = true;
        RegisterToManager();

        // 제일 첫번째 자식이 리더이다.
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Playable>();
            if (cha != null)
            {
                if (i == 0)
                    CurrentCharacter = cha;

                cha.Spawn();

                Characters.Add(cha);
            }
        }

        // 리더의 패시브 스킬이 있으면 적용
        if (Characters[0].PassiveSkill != null)
            Characters[0].PassiveSkill.Apply(Character.GetPassiveIndex(Characters[0].Code));

        // 프리로드 할 꺼 있으면 해주기
        Characters.ForEach(cha =>
        {
            if (cha.Preloads != null)
            {
                var playablePreload = cha.Preloads as PlayablePreloadSettings;
                playablePreload.Instantitate(cha);
            }
                
        });

        m_RigidbodyControlCoroutine = ControlRigidbodyCoroutine();
        m_GetInputCoroutine = GetInputCoroutine();
        m_TransformControlCoroutine = ControlTransformCoroutine();

        Moveable = true;
    }

    void RegisterToManager()
    {
        switch (GameManager.SceneCode)
        {
            case SceneCode.Lobby:
                LobbyManager.Instance.Player = this;
                break;
            default:
                StageManager.Instance.Player = this;
                break;
        }
    }

    void ReleaseFromManager()
    {
        switch (GameManager.SceneCode)
        {
            case SceneCode.Lobby:
                LobbyManager.Instance.Player = null;
                break;
            default:
                StageManager.Instance.Player = null;
                break;
        }
    }
    #endregion

    #region 캐릭터 이동

    #region 디바이스 활용
    public bool Controlable
    {
        set
        {
            if (value)
            {
                StartCoroutine(m_GetInputCoroutine);
                StartCoroutine(m_RigidbodyControlCoroutine);
            }
            else
            {
                StopCoroutine(m_GetInputCoroutine);
                StopCoroutine(m_RigidbodyControlCoroutine);
            }

        }
    }
    [ReadOnly] public bool Moveable;
    public Vector3 MoveVector { get; private set; }
    public Vector3 RotateVector { get; private set; }

    IEnumerator m_RigidbodyControlCoroutine;
    IEnumerator m_TransformControlCoroutine;
    IEnumerator m_GetInputCoroutine;

    const float CHARCTER_ROTATE_SPEED = 20f;
    IEnumerator GetInputCoroutine()
    {
        while (true)
        {
            // 캐릭터 움직이기
            var controllerInput = GameManager.InputSystem.LeftStickInput;
            var cam = StageManager.Instance.MainCam;
            if (cam != null)
            {
                // 이동값
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * controllerInput.y + cameraRight * controllerInput.x;

                // 회전값
                float angle = Vector3.SignedAngle(CurrentCharacter.transform.forward, moveVector, Vector3.up);
                Vector3 angularVelocity = new Vector3(0, angle, 0);

                // 인풋 벡터
                if (Moveable)
                {
                    MoveVector = moveVector;
                    RotateVector = angularVelocity;
                }
                else
                {
                    MoveVector = Vector3.zero;
                    RotateVector = Vector3.zero;
                }
            }
            yield return null;
        }
    }

    IEnumerator ControlTransformCoroutine()
    {
        while (true)
        {
            bool skip = false;

            if (MoveVector.magnitude == 0)
                skip = true;
            if (!Moveable)
                skip = true;
            if (CurrentCharacter.Hp <= 0)
                skip = true;

            if (!skip)
            {
                // 회전
                CurrentCharacter.transform.forward = Vector3.Lerp(CurrentCharacter.transform.forward, MoveVector.normalized, Time.deltaTime * CHARCTER_ROTATE_SPEED);

                // 이동
                CurrentCharacter.transform.position += MoveVector * Time.deltaTime * CurrentCharacter.MoveSpeed;
            }
                

            yield return null;
        }
    }

    IEnumerator ControlRigidbodyCoroutine()
    {
        while (true)
        {
            CurrentCharacter.Rigidbody.MoveRotation(CurrentCharacter.Rigidbody.rotation * Quaternion.Euler(RotateVector));

            bool movePositionSkip = false;

            #region 예외처리
            if (MoveVector.magnitude == 0)
                movePositionSkip = true;
            if (!Moveable)
                movePositionSkip = true;
            #endregion

            if (!movePositionSkip)
            {
                // 이동가능하면 움직이자
                Vector3 desired = CurrentCharacter.transform.position + (MoveVector * CurrentCharacter.MoveSpeed * Time.fixedDeltaTime);
                CurrentCharacter.Rigidbody.MovePosition(desired);
            }

            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region 목적지로 캐릭터 무조건 이동
    bool m_SmoothlyMoving;

    /// <summary> 목적지로 캐릭터가 정해진 시간안에 이동하게 합니다. </summary>
    public void SmoothlyMovingTo(Vector3 destination, float desiredTime, Action onArriveCallback = null)
    {
        if (m_SmoothlyMoving)
            return;

        StartCoroutine(SmoothlyMovingCoroutine(destination, desiredTime, onArriveCallback));
    }

    IEnumerator SmoothlyMovingCoroutine(Vector3 destination, float desiredTime, Action onArriveCallback = null)
    {
        float t = 0f;
        m_SmoothlyMoving = true;

        // 현재 캐릭터로부터 목적지까지의 거리벡터
        Vector3 adjust = destination - CurrentCharacter.transform.position;

        while (t < 1)
        {
            t += Time.fixedDeltaTime / desiredTime;

            // Force가 계속해서 더해지는 현상 방지하기 위함
            CurrentCharacter.Rigidbody.velocity = Vector3.zero;
            CurrentCharacter.Rigidbody.AddForce(adjust / desiredTime, ForceMode.VelocityChange);

            yield return new WaitForFixedUpdate();
        }
        m_SmoothlyMoving = false;
        CurrentCharacter.Rigidbody.velocity = Vector3.zero;

        onArriveCallback?.Invoke();
    }
    #endregion

    #endregion

    #region 캐릭터 스왑
    float m_SwapCoolTimer = 0f;
    const float SWAP_COOLTIME = 2f;

    public void SwapCharacter(Playable swapCharacter)
    {
        if (m_SwapCoolTimer != 0)
            return;

        var sm = StageManager.Instance;
        if (sm == null)
            return;

        var changeCharacter = swapCharacter;
        if (changeCharacter.Hp <= 0)
            return;

        // 캐릭터 버튼에 스왑 쿨타임 적용
        var inGameUi = GameManager.UISystem.CurrentWindow as InGameUI;
        StartCoroutine(SwapCoolTimeCoroutine(inGameUi.CharacterButtonDisplays));

        // 기존 캐릭터에 대한 처리
        var prevCharcter = CurrentCharacter;
        prevCharcter.gameObject.SetActive(false);
        prevCharcter.DisposeEvents();

        // 기존 캐릭터 버튼을 켜준다.
        var prevCharacterButton = inGameUi.CharacterButtonDisplays.Find(button => button.ConnectCharacter.Equals(prevCharcter));
        prevCharacterButton.gameObject.SetActive(true);
        prevCharacterButton.SetData(prevCharcter);

        // 스왑할 캐릭터의 위치
        Vector3 spawnPosition = CurrentCharacter.transform.position;
        Quaternion spawnRotation = CurrentCharacter.transform.rotation;
        changeCharacter.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        // 스왑할 캐릭터 카메라 및 업데이트 활성화
        changeCharacter.gameObject.SetActive(true);
        changeCharacter.SetUpdate(true);
        StageManager.Instance.FreeLookCam.Follow = changeCharacter.transform;
        StageManager.Instance.FreeLookCam.LookAt = changeCharacter.transform;

        // 스왑할 캐릭터의 CharacterButtonUI
        var changeCharacterButton = inGameUi.CharacterButtonDisplays.Find(button => button.ConnectCharacter.Equals(changeCharacter));
        changeCharacterButton.gameObject.SetActive(false);

        // 인게임 UI의 공격버튼 및 HP/SP 슬라이더를 스왑할 캐릭터의 것으로
        inGameUi.SettingSkillButtons(changeCharacter);
        inGameUi.SettingSliders(changeCharacter);

        // 몬스터들의 타겟은 스왑한 캐릭터로 바뀐다.
        StageManager.Instance.Monsters.ForEach(mob => mob.Target = changeCharacter);

        // 캐릭터 스왑 이펙트
        var effect = StageManager.Instance.PoolSystem.Load<CharacterSwapEffect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/FX_LevelUp_01");
        effect.transform.position = changeCharacter.transform.position;


        CurrentCharacter = changeCharacter;
    }

    IEnumerator SwapCoolTimeCoroutine(List<CharacterButtonUI> buttons)
    {
        foreach (var button in buttons)
            button.CoolTime.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < SWAP_COOLTIME)
        {
            timer += Time.deltaTime;

            m_SwapCoolTimer = SWAP_COOLTIME - timer;

            foreach (var button in buttons)
            {
                button.CoolTime.fillAmount = 1 - (timer / SWAP_COOLTIME);
            }

            yield return null;
        }
        m_SwapCoolTimer = 0f;

        foreach (var button in buttons)
        {
            button.CoolTime.fillAmount = 0f;
            button.CoolTime.gameObject.SetActive(false);
        }
    }
    #endregion

    #region X버튼 입력시
    /// <summary> 대쉬버튼(X) 대신 이걸 호출하여 대쉬를 한다. </summary>
    public void InputX(SkillButtonUI skillButtonUI)
    {
        if (CurrentCharacter.CanXInput() == false)
            return;

        var skillData = Character.GetSkillData(Character.GetXInputDataIndex(CurrentCharacter.Code));
        if (skillData.Stack != 0)
        {
            // 스택을 다 쓰면 스킬을 사용할 수 없다.
            if (CurrentCharacter.XStack == 0)
                return;

            // 이 버튼을 눌러야 대쉬가 나가도록 되어있음
            GameManager.InputSystem.PressXButton = true;

            // Sp 소비
            CurrentCharacter.Sp -= skillData.SpCost;

            CurrentCharacter.XStack--;
            CurrentCharacter.OnXInput();

            if (skillButtonUI)
                skillButtonUI.OnStackConsume();

            if (CurrentCharacter.XStack < skillData.Stack)
                StartCoroutine(ChargeXStackCoroutine(CurrentCharacter, skillButtonUI));
        }
        else
        {
            GameManager.InputSystem.PressXButton = true;
            CurrentCharacter.OnXInput();

            CurrentCharacter.Sp -= skillData.SpCost;
        }

    }

    IEnumerator ChargeXStackCoroutine(Playable character, SkillButtonUI skillButtonUI)
    {
        float timer = 0f;
        var skillData = Character.GetSkillData(Character.GetXInputDataIndex(character.Code));
        float duration = skillData.CoolTime;
        float maxStack = skillData.Stack;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        if (character.XStack < maxStack)
        {
            character.XStack++;
            if (skillButtonUI != null && CurrentCharacter.Equals(character))
                skillButtonUI.OnStackCharge(character.XStack);
        }
    }
    #endregion

    #region A버튼 입력시
    /// <summary> 공격버튼(A) 대신 이걸 호출하여 대쉬를 한다. </summary>
    public void InputA(SkillButtonUI skillButtonUI)
    {
        if (CurrentCharacter.CanAInput() == false)
            return;

        GameManager.InputSystem.PressAButton = true;
        CurrentCharacter.OnAInput();
    }
    #endregion

    #region B버튼 입력시
    /// <summary> 궁극기버튼(B) 대신 이걸 호출하여 궁극기를 한다. </summary>
    public void InputB(SkillButtonUI skillButtonUI)
    {
        if (CurrentCharacter.CanBInput() == false)
            return;

        // 치트
        if (GameManager.CheatSettings.FreeSkill)
        {
            GameManager.InputSystem.PressBButton = true;
            CurrentCharacter.OnBInput();

            return;
        }

        // --------------------------------------------------

        var currentCharacter = CurrentCharacter;
        var skillData = Character.GetSkillData(Character.GetBInputDataIndex(currentCharacter.Code));
        
        if (skillData.Stack != 0)
        {
            // 스택을 다 쓰면 스킬을 사용할 수 없다.
            if (CurrentCharacter.BStack == 0)
                return;

            GameManager.InputSystem.PressBButton = true;

            currentCharacter.Sp -= skillData.SpCost;

            currentCharacter.BStack--;
            CurrentCharacter.OnBInput();

            skillButtonUI.OnStackConsume();

            StartCoroutine(ChargeInputBCooldownCoroutine(currentCharacter, skillButtonUI, () =>
            {
                float maxStack = skillData.Stack;
                currentCharacter.BStack++;
                skillButtonUI.OnStackCharge(currentCharacter.BStack);
            }));
        }
        else
        {
            GameManager.InputSystem.PressBButton = true;
            CurrentCharacter.OnBInput();

            currentCharacter.Sp -= skillData.SpCost;

            StartCoroutine(ChargeInputBCooldownCoroutine(currentCharacter, skillButtonUI));
        }

        
    }

    IEnumerator ChargeInputBCooldownCoroutine(Playable character, SkillButtonUI skillButtonUI, Action onCooldownEndCallback = null)
    {
        float timer = 0f;
        float progress = 0f;
        var skillData = Character.GetSkillData(Character.GetBInputDataIndex(CurrentCharacter.Code));
        float duration = skillData.CoolTime;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;

            // UI 표기
            progress = timer / duration;
            if (skillButtonUI != null && CurrentCharacter.Equals(character))
                skillButtonUI.CoolTimeBackground.fillAmount = 1 - progress;

            // 쿨타임 데이터
            character.BCoolTime = duration - timer;

            yield return null;
        }
        character.BCoolTime = 0f;

        onCooldownEndCallback?.Invoke(); 
    }
    #endregion

    #region Y버튼 입력시
    /// <summary> 점프버튼(Y) 대신 이걸 호출하여 궁극기를 한다. </summary>
    public void InputY(SkillButtonUI skillButtonUI = null)
    {
        if (CurrentCharacter.CanYInput() == false)
            return;

        // 이 버튼을 눌러야 점프가 나가도록 되어있음
        GameManager.InputSystem.PressYButton = true;
        CurrentCharacter.OnYInput();
    }
    #endregion
}
