using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuMechanism : MonoSingleton<MainMenuMechanism>
{
    public Transform CameraPosition;
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

    const float m_CAMERA_MOVE_SPEED = 0.5F;
    IEnumerator m_CheckUserClickingTheCharacterCoroutine;

    private void Start()
    {
        m_CheckUserClickingTheCharacterCoroutine = CheckingUserClickCharacterCoroutine();
    }

    public void MovingCamera()
    {
        StartCoroutine(MovingCameraCoroutine());
    }

    IEnumerator MovingCameraCoroutine()
    {
        float timer = 0f;
        float sensitivity = 0f;
        Camera mainCam = GameManager.Instance.MainCam;
        Vector3 goalPosition = CameraPosition.transform.position;

        while (timer < 4f)
        {
            timer += Time.deltaTime;
            sensitivity += timer * 0.001f;
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, goalPosition, sensitivity * m_CAMERA_MOVE_SPEED);
            yield return null;
        }
        if (LoadingTitleMechanism.Instance != null)
            Destroy(LoadingTitleMechanism.Instance.gameObject);

        GameManager.Instance.UISystem.OpenWindow(UIType.MainMenu);
    }

    IEnumerator CheckingUserClickCharacterCoroutine()
    {
        while (true)
        {
            // ¿ÞÂÊ Å¬¸¯½Ã
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                RaycastHit hitInfo;
                Ray ray = GameManager.Instance.MainCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
                {
                    var player = GameManager.Instance.Player;
                    var character = player.CurrentCharacter;
                    if (hitInfo.collider.gameObject.Equals(character.gameObject))
                    {
                        int random = Random.Range(0, character.AnimationsWhenUserClick.Count);
                        AniType randomAni = character.AnimationsWhenUserClick[random];

                        player.Commands.Enqueue(randomAni);
                    }
                }
            }

            yield return null;
        }

    }
}
