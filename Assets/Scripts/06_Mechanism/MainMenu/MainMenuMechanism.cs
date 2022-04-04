using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuMechanism : Register
{
    public Transform CameraPosition;

    const float CAMERA_MOVE_SPEED = 0.5F;

    public override void RegisterToGameManager()
    {
        GameManager.Instance.Mechanism_MainMenu = this;
    }

    public override void ReleaseToGameManager()
    {
        GameManager.Instance.Mechanism_MainMenu = null;
    }

    public void StartDirecting()
    {
        StartCoroutine(MakeCameraMovingToPosition());
    }

    IEnumerator MakeCameraMovingToPosition()
    {
        float timer = 0f;
        float sensitivity = 0f;
        Camera mainCam = GameManager.Instance.MainCam;
        Vector3 goalPosition = CameraPosition.transform.position;

        while (timer < 4f)
        {
            timer += Time.deltaTime;
            sensitivity += timer * 0.001f;
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, goalPosition, sensitivity * CAMERA_MOVE_SPEED);
            yield return null;
        }
        GameManager.Instance.UI.OpenWindow(UIType.MainMenu);
        StartCoroutine(CheckingUserClickCharacter());
    }

    IEnumerator CheckingUserClickCharacter()
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

                        character.Animator.SetInteger(character.ANITYPE_HASHCODE, (int)randomAni);
                    }
                }
            }
            yield return null;
        }
        
    }
}
