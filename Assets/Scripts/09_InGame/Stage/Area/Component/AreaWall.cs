using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaWall : AreaComponent
{
    void OnTriggerStay(Collider other)
    {
        // TODO: 캐릭터 반대방향으로 밀어내기
        var player = GameManager.Instance.Player;
        Vector3 desired = player.transform.position - player.MoveVector;
        float movespeed = player.CurrentCharacter.Speed;

        player.transform.position = Vector3.Lerp(player.transform.position, desired, Time.deltaTime * movespeed);
    }
}
