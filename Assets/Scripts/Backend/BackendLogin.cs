using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendLogin
{
    private static BackendLogin _instance = null;

    public static BackendLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendLogin();
            }

            return _instance;
        }
    }

    public string CustomSignUp(string id, string pw)
    {
        Debug.Log("회원가입을 요청합니다.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. : " + bro);
            return "회원 가입 성공!";
        }
        else
        {
            Debug.LogError("회원가입에 실패했습니다. : " + bro);
            return "회원 가입에 실패했습니다. 아이디나 비밀번호를 확인해주세요.";
        }

        return null;
    }

    public string CustomLogin(string id, string pw)
    {
        Debug.Log("로그인을 요청합니다.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공했습니다. : " + bro);
            return "로그인 성공!";
        }
        else
        {
            Debug.LogError("로그인이 실패했습니다. : " + bro);
            return "아이디나 비밀번호가 일치하지 않습니다.";
        }

        return null;
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("닉네임 변경을 요청합니다.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경에 성공했습니다 : " + bro);
        }
        else
        {
            Debug.LogError("닉네임 변경에 실패했습니다 : " + bro);
        }
    }
    
    public string GetNickName() 
    {
        var bro = Backend.BMember.GetUserInfo();
        
        if (bro.IsSuccess())
        {
            if (bro.GetReturnValuetoJSON()["row"]["nickname"] != null)
            {
                return bro.GetReturnValuetoJSON()["row"]["nickname"].ToString();
            }
            else
            {
                return "닉네임이 없습니다.";
            }
        }
        else
        {
            Debug.LogError("사용자 정보 조회 실패: " + bro);
            return "사용자 정보 조회 실패";
        }
    }
}