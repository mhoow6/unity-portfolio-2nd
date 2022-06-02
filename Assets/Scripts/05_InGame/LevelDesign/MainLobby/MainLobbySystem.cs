using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DatabaseSystem;

public class MainLobbySystem : MonoBehaviour
{
    public Transform CameraPosition;
    public Transform PlayerSpawnPosition;
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
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.MainLobbySystem = null;
    }

    public void Init()
    {
        SpawnMainCharacter();
        StartCoroutine(MovingCameraCoroutine());
    }

    public void FastInit()
    {
        SpawnMainCharacter();

        LobbyManager.Instance.MainCam = LobbyCamera;
        LobbyManager.Instance.MainCam.transform.position = CameraPosition.transform.position;
        Destroy(LobbyManager.Instance.LoadingTitleSystem.gameObject);
        GameManager.UISystem.OpenWindow(UIType.MainLobby);
    }

    IEnumerator MovingCameraCoroutine()
    {
        float timer = 0f;
        float sensitivity = 0f;
        Camera mainCam = LobbyManager.Instance.MainCam;
        Vector3 goalPosition = CameraPosition.transform.position;

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
            // ¿ÞÂÊ Å¬¸¯½Ã
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

        var leader = Character.Get(main, player.transform, resourcePath) as Playable;
        leader.gameObject.SetActive(true);
        leader.transform.position = PlayerSpawnPosition.position;
        leader.transform.rotation = PlayerSpawnPosition.rotation;

        var row = TableManager.Instance.CharacterTable.Find(character => character.Code == main);
        leader.SetLobbyAnimations(row.LobbyAnimatorPath);

        player.Init();
    }
}
