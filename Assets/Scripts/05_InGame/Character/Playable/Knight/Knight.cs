using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Playable
{
    public override ObjectCode Code => ObjectCode.CHAR_Knight;

    public override void OnXInput()
    {
        // �̵��ӵ� ���� ���� (�ڱ� �ڽŸ�)
    }

    public override void OnBInput()
    {
        // ���� ����Ʈ�� �Բ� ���� ���� ���� (ĳ���� ��ü�ÿ��� ����)
    }
}
