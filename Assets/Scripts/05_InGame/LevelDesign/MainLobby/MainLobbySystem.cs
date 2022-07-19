using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DatabaseSystem;

public class MainLobbySystem : MonoBehaviour, IGameEventListener
{
    public Transform LobbyUICameraPosition;
    public Transform PlayerSpawnPosition;
    public Transform CharacterUICameraPosition;
    public Transform CharacterUICharacterSpawnTransform;
    public Camera LobbyCamera;
    public bool CheckUserClickingTheCharacter
    {
        set
        {
            if (value)
                StartCoroutine(m_CheckUserClickingTheCharacterCoroutine);
            else
                StopCoroutine(m_CheckUserClickingTheCharacterCoroutine);
        }
    }

    const float CAMERA_MOVE_SPEED = 0.5F;
    IEnumerator m_CheckUserClickingTheCharacterCoroutine;

    void Awake()
    {
        m_CheckUserClickingTheCharacterCoroutine = CheckingUserClickCharacterCoroutine();

        GameEventSystem.AddListener(this);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.MainLobbySystem = null;

        GameEventSystem.RemoveListener(this);
    }

    public void Init()
    {
        SpawnMainCharacter();
        StartCoroutine(MovingCameraCoroutine());
    }

    public void FastInit()
    {
        SpawnMainCharacter();

        var lm = LobbyManager.Instance;

        lm.MainCam = LobbyCamera;
        lm.MainCam.transform.position = LobbyUICameraPosition.transform.position;

        Destroy(lm.LoadingTitleSystem.gameObject);
        GameManager.UISystem.OpenWindow(UIType.MainLobby);
    }

    IEnumerator MovingCameraCoroutine()
    {
        float timer = 0f;
        float sensitivity = 0f;
        Camera mainCam = LobbyManager.Instance.MainCam;
        Vector3 goalPosition = LobbyUICameraPosition.transform.position;

        while (timer < 4f)
        {
            timer += Time.deltaTime;
            sensitivity += timer * 0.001f;
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, goalPosition, sensitivity * CAMERA_MOVE_SPEED);
            yield return null;
        }

        LobbyManager.Instance.MainCam = LobbyCamera;
        Destroy(LobbyManager.Instance.LoadingTitleSystem.gameObject);
        GameManager.UISystem.OpenWindow(UIType.MainLobby);
    }

    IEnumerator CheckingUserClickCharacterCoroutine()
    {
        while (true)
        {
            // 왼쪽 클릭시
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                RaycastHit hitInfo;
                Ray ray = LobbyManager.Instance.MainCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
                {
                    var player = LobbyManager.Instance.Player;
                    var character = player.CurrentCharacter as Playable;
                    if (hitInfo.collider.gameObject.Equals(character.gameObject))
                    {
                        int random = Random.Range(0, character.LobbyAnimations.Count);
                        AniType randomAni = character.LobbyAnimations[random];

                        player.AnimationJobs.Enqueue(randomAni);
                    }
                }
            }

            yield return null;
        }

    }

    void SpawnMainCharacter()
    {
        var player = new GameObject("Player").AddComponent<Player>();
        player.gameObject.SetActive(true);
        player.transform.SetParent(PlayerSpawnPosition);

        var main = GameManager.PlayerData.MainMenuCharacter;
        var resourcePath = GameManager.GameDevelopSettings.CharacterResourcePath;

        var leader = Character.Get(main, player.transform) as Playable;
        leader.gameObject.SetActive(true);
        leader.transform.position = PlayerSpawnPosition.position;
        leader.transform.rotation = PlayerSpawnPosition.rotation;

        var row = TableManager.Instance.CharacterTable.Find(character => character.Code == main);
        leader.SetLobbyAnimations(row.LobbyAnimatorPath);

        player.Init();
    }
    public void Listen(GameEvent gameEvent)
    {
        
    }

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        switch (gameEvent)
        {
            case GameEvent.LOBBY_SwapCharacter:
                if (args.Length != 1)
                    return;

                ObjectCode selectedCharacter = (ObjectCode)args[0];

                if (selectedCharacter == ObjectCode.NONE)
                    return;

                // 이전에 캐릭터가 이미 생성되면 삭제처리
                for (int i = 0; i < CharacterUICharacterSpawnTransform.childCount; i++)
                {
                    var child = CharacterUICharacterSpawnTransform.GetChild(i);
                    Destroy(child.gameObject);
                }

                // 정해진 위치에서 캐릭터를 볼 수 있도록 캐릭터 생성
                var character = Character.Get(selectedCharacter, CharacterUICharacterSpawnTransform);
                character.Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{GameManager.GameDevelopSettings.AnimationControllerResourcePath}/Lobby_CharacterUI_Character");
                character.transform.SetPositionAndRotation(CharacterUICharacterSpawnTransform.position, CharacterUICharacterSpawnTransform.rotation);
                break;
        }
    }
}
