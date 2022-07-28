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
        // ���⿡�� �����Ӻ��� �ִϸ��̼��� �ϳ��� �޾ƿ����� ó��
        if (AnimationJobs.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());
    }

    void OnDestroy()
    {
        ReleaseFromManager();
    }

    #region �ʱ�ȭ
    bool m_Init = false;

    /// <summary> ���ο� ������ �÷��̾� �ʱ�ȭ�ϱ� </summary>
    public void Init()
    {
        if (m_Init)
            return;
        m_Init = true;
        RegisterToManager();

        // ���� ù��° �ڽ��� �����̴�.
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

        // ������ �нú� ��ų�� ������ ����
        if (Characters[0].PassiveSkill != null)
            Characters[0].PassiveSkill.Apply(Character.GetPassiveIndex(Characters[0].Code));

        // �����ε� �� �� ������ ���ֱ�
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

    #region ĳ���� �̵�

    #region ����̽� Ȱ��
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
            // ĳ���� �����̱�
            var controllerInput = GameManager.InputSystem.LeftStickInput;
            var cam = StageManager.Instance.MainCam;
            if (cam != null)
            {
                // �̵���
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * controllerInput.y + cameraRight * controllerInput.x;

                // ȸ����
                float angle = Vector3.SignedAngle(CurrentCharacter.transform.forward, moveVector, Vector3.up);
                Vector3 angularVelocity = new Vector3(0, angle, 0);

                // ��ǲ ����
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
                // ȸ��
                CurrentCharacter.transform.forward = Vector3.Lerp(CurrentCharacter.transform.forward, MoveVector.normalized, Time.deltaTime * CHARCTER_ROTATE_SPEED);

                // �̵�
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

            #region ����ó��
            if (MoveVector.magnitude == 0)
                movePositionSkip = true;
            if (!Moveable)
                movePositionSkip = true;
            #endregion

            if (!movePositionSkip)
            {
                // �̵������ϸ� ��������
                Vector3 desired = CurrentCharacter.transform.position + (MoveVector * CurrentCharacter.MoveSpeed * Time.fixedDeltaTime);
                CurrentCharacter.Rigidbody.MovePosition(desired);
            }

            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region �������� ĳ���� ������ �̵�
    bool m_SmoothlyMoving;

    /// <summary> �������� ĳ���Ͱ� ������ �ð��ȿ� �̵��ϰ� �մϴ�. </summary>
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

        // ���� ĳ���ͷκ��� ������������ �Ÿ�����
        Vector3 adjust = destination - CurrentCharacter.transform.position;

        while (t < 1)
        {
            t += Time.fixedDeltaTime / desiredTime;

            // Force�� ����ؼ� �������� ���� �����ϱ� ����
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

    #region ĳ���� ����
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

        // ĳ���� ��ư�� ���� ��Ÿ�� ����
        var inGameUi = GameManager.UISystem.CurrentWindow as InGameUI;
        StartCoroutine(SwapCoolTimeCoroutine(inGameUi.CharacterButtonDisplays));

        // ���� ĳ���Ϳ� ���� ó��
        var prevCharcter = CurrentCharacter;
        prevCharcter.gameObject.SetActive(false);
        prevCharcter.DisposeEvents();

        // ���� ĳ���� ��ư�� ���ش�.
        var prevCharacterButton = inGameUi.CharacterButtonDisplays.Find(button => button.ConnectCharacter.Equals(prevCharcter));
        prevCharacterButton.gameObject.SetActive(true);
        prevCharacterButton.SetData(prevCharcter);

        // ������ ĳ������ ��ġ
        Vector3 spawnPosition = CurrentCharacter.transform.position;
        Quaternion spawnRotation = CurrentCharacter.transform.rotation;
        changeCharacter.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        // ������ ĳ���� ī�޶� �� ������Ʈ Ȱ��ȭ
        changeCharacter.gameObject.SetActive(true);
        changeCharacter.SetUpdate(true);
        StageManager.Instance.FreeLookCam.Follow = changeCharacter.transform;
        StageManager.Instance.FreeLookCam.LookAt = changeCharacter.transform;

        // ������ ĳ������ CharacterButtonUI
        var changeCharacterButton = inGameUi.CharacterButtonDisplays.Find(button => button.ConnectCharacter.Equals(changeCharacter));
        changeCharacterButton.gameObject.SetActive(false);

        // �ΰ��� UI�� ���ݹ�ư �� HP/SP �����̴��� ������ ĳ������ ������
        inGameUi.SettingSkillButtons(changeCharacter);
        inGameUi.SettingSliders(changeCharacter);

        // ���͵��� Ÿ���� ������ ĳ���ͷ� �ٲ��.
        StageManager.Instance.Monsters.ForEach(mob => mob.Target = changeCharacter);

        // ĳ���� ���� ����Ʈ
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

    #region X��ư �Է½�
    /// <summary> �뽬��ư(X) ��� �̰� ȣ���Ͽ� �뽬�� �Ѵ�. </summary>
    public void InputX(SkillButtonUI skillButtonUI)
    {
        if (CurrentCharacter.CanXInput() == false)
            return;

        var skillData = Character.GetSkillData(Character.GetXInputDataIndex(CurrentCharacter.Code));
        if (skillData.Stack != 0)
        {
            // ������ �� ���� ��ų�� ����� �� ����.
            if (CurrentCharacter.XStack == 0)
                return;

            // �� ��ư�� ������ �뽬�� �������� �Ǿ�����
            GameManager.InputSystem.PressXButton = true;

            // Sp �Һ�
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

    #region A��ư �Է½�
    /// <summary> ���ݹ�ư(A) ��� �̰� ȣ���Ͽ� �뽬�� �Ѵ�. </summary>
    public void InputA(SkillButtonUI skillButtonUI)
    {
        if (CurrentCharacter.CanAInput() == false)
            return;

        GameManager.InputSystem.PressAButton = true;
        CurrentCharacter.OnAInput();
    }
    #endregion

    #region B��ư �Է½�
    /// <summary> �ñر��ư(B) ��� �̰� ȣ���Ͽ� �ñر⸦ �Ѵ�. </summary>
    public void InputB(SkillButtonUI skillButtonUI)
    {
        if (CurrentCharacter.CanBInput() == false)
            return;

        // ġƮ
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
            // ������ �� ���� ��ų�� ����� �� ����.
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

            // UI ǥ��
            progress = timer / duration;
            if (skillButtonUI != null && CurrentCharacter.Equals(character))
                skillButtonUI.CoolTimeBackground.fillAmount = 1 - progress;

            // ��Ÿ�� ������
            character.BCoolTime = duration - timer;

            yield return null;
        }
        character.BCoolTime = 0f;

        onCooldownEndCallback?.Invoke(); 
    }
    #endregion

    #region Y��ư �Է½�
    /// <summary> ������ư(Y) ��� �̰� ȣ���Ͽ� �ñر⸦ �Ѵ�. </summary>
    public void InputY(SkillButtonUI skillButtonUI = null)
    {
        if (CurrentCharacter.CanYInput() == false)
            return;

        // �� ��ư�� ������ ������ �������� �Ǿ�����
        GameManager.InputSystem.PressYButton = true;
        CurrentCharacter.OnYInput();
    }
    #endregion
}
