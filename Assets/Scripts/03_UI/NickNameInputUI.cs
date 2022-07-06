using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using System.Text.RegularExpressions;
using System.Text;

public class NickNameInputUI : ConfirmUI
{
    public InputField InputField;
    public override UIType Type => UIType.NickNameInput;

    const int MAXIMUM_NICKNAME_BYTE = 16;

    public void OnXBtnClick()
    {
        GameManager.UISystem.CloseWindow();
    }

    public override void OnClosed()
    {
        if (string.IsNullOrEmpty(InputField.text))
        {
            var table = TableManager.Instance.RandomNicknameTable;
            int randomIndx = Random.Range(0, table.Count);
            GameManager.PlayerData.NickName = table[randomIndx].RandomNickname;
        }
        InputField.text = string.Empty;
    }

    public override void OnOpened()
    {
        // 메인 카메라 키기
        LobbyManager.Instance.MainCam.gameObject.SetActive(true);

        SetData(
            "닉네임 입력(1 ~ 8글자)",
            () =>
            {
                if (IsVaildNickName(InputField.text))
                {
                    GameManager.PlayerData.NickName = InputField.text;
                    GameManager.UISystem.CloseWindow();
                }
                    
            });
    }

    bool IsVaildNickName(string nickname)
    {
        // 최소 최대 자릿 수 검사
        int byteCount = Encoding.UTF8.GetBytes(nickname).Length;
        if (byteCount < 1 || byteCount > MAXIMUM_NICKNAME_BYTE)
        {
            var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
            warning.SetData("닉네임이 1글자보다 적거나 8글자보다 많습니다.");
            InputField.text = string.Empty;
            return false;
        }

        // 특수문자 검사
        //if (Regex.IsMatch(InputField.text, @"[^0-9a-zA-Z]+"))
        //{
        //    var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
        //    warning.SetData("특수문자를 포함시킬 수 없습니다.");
        //    InputField.text = string.Empty;
        //    return false;
        //}    

        // 비속어 검사
        var table = TableManager.Instance.SlangTable;
        foreach (var row in table)
        {
            if (nickname.Contains(row.SlangWord))
            {
                var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                warning.SetData("비속어를 포함할 수 없습니다.");
                InputField.text = string.Empty;
                return false;
            }
        }

        return true;
    }
}
