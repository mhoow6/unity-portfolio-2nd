using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using System.Text.RegularExpressions;

public class NickNameInputUI : ConfirmUI
{
    public InputField InputField;
    public override UIType Type => UIType.NickNameInput;

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
        // �ڸ� �� �˻�
        if (nickname.Length < 1 || nickname.Length > 8)
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
