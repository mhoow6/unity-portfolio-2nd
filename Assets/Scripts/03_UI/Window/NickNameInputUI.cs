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
            "´Ð³×ÀÓ ÀÔ·Â(1 ~ 8±ÛÀÚ)",
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
        // ÃÖ¼Ò ÃÖ´ë ÀÚ¸´ ¼ö °Ë»ç
        int byteCount = Encoding.UTF8.GetBytes(nickname).Length;
        if (byteCount < 1 || byteCount > MAXIMUM_NICKNAME_BYTE)
        {
            Debug.Log("´Ð³×ÀÓÀÌ 1±ÛÀÚº¸´Ù Àû°Å³ª 8±ÛÀÚº¸´Ù ¸¹½À´Ï´Ù.");
            InputField.text = string.Empty;
            return false;
        }

        // Æ¯¼ö¹®ÀÚ °Ë»ç
        if (Regex.IsMatch(InputField.text, @"[^a-zA-Z0-9°¡-ÆR]"))
        {
            Debug.Log("Æ¯¼ö¹®ÀÚ¸¦ Æ÷ÇÔ½ÃÅ³ ¼ö ¾ø½À´Ï´Ù.");
            InputField.text = string.Empty;
            return false;
        }    

        // ºñ¼Ó¾î °Ë»ç
        var table = TableManager.Instance.SlangTable;
        foreach (var row in table)
        {
            if (nickname.Contains(row.SlangWord))
            {
                // TODO: ¸Þ½ÃÁö ¶ç¿ì±â
                Debug.Log("ºñ¼Ó¾î¸¦ Æ÷ÇÔÇÒ ¼ö ¾ø½À´Ï´Ù");
                InputField.text = string.Empty;
                return false;
            }
        }

        return true;
    }
}
