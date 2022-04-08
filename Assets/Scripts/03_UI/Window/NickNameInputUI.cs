using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using System.Text.RegularExpressions;
using System.Text;

public class NickNameInputUI : ConfirmUI
{
    public InputField InputField;
    public override UIType Type => UIType.NickNameInput;

    const int MAXIMUM_NICKNAME_BYTE = 16;

    public void OnXBtnClick()
    {
        GameManager.Instance.UISystem.CloseWindow();
    }

    public override void OnClosed()
    {
        if (string.IsNullOrEmpty(InputField.text))
        {
            var table = TableManager.Instance.RandomNicknameTable;
            int randomIndx = Random.Range(0, table.Count);
            GameManager.Instance.PlayerData.NickName = table[randomIndx].RandomNickname;
        }
        InputField.text = string.Empty;
    }

    public override void OnOpened()
    {
        SetData(
            "�г��� �Է�(1 ~ 8����)",
            () =>
            {
                if (IsVaildNickName(InputField.text))
                {
                    GameManager.Instance.PlayerData.NickName = InputField.text;
                    GameManager.Instance.UISystem.CloseWindow();
                }
                    
            });
    }

    bool IsVaildNickName(string nickname)
    {
        // �ּ� �ִ� �ڸ� �� �˻�
        int byteCount = Encoding.UTF8.GetBytes(nickname).Length;
        if (byteCount < 1 || byteCount > MAXIMUM_NICKNAME_BYTE)
        {
            Debug.Log("�г����� 1���ں��� ���ų� 8���ں��� �����ϴ�.");
            InputField.text = string.Empty;
            return false;
        }

        // Ư������ �˻�
        if (Regex.IsMatch(InputField.text, @"[^a-zA-Z0-9��-�R]"))
        {
            Debug.Log("Ư�����ڸ� ���Խ�ų �� �����ϴ�.");
            InputField.text = string.Empty;
            return false;
        }    

        // ��Ӿ� �˻�
        var table = TableManager.Instance.SlangTable;
        foreach (var row in table)
        {
            if (nickname.Contains(row.SlangWord))
            {
                // TODO: �޽��� ����
                Debug.Log("��Ӿ ������ �� �����ϴ�");
                InputField.text = string.Empty;
                return false;
            }
        }

        return true;
    }
}
