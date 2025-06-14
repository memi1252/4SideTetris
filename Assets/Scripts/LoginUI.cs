using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private InputField userid;
    [SerializeField] private InputField password;
    [SerializeField] private InputField nickname;
    [SerializeField] private Text StatusText;

    public void Logint()
    {
        StatusText.text = BackendLogin.Instance.CustomLogin(userid.text, password.text);
        if (StatusText.text == "로그인 성공!")
        {
            userid.gameObject.SetActive(false);
            password.gameObject.SetActive(false);
        }

        CheckAndCreateNickname();
    }

    public void SignUp()
    {
        StatusText.text = BackendLogin.Instance.CustomSignUp(userid.text, password.text);
    }

    public void CreateNickname()
    {
        var where = new Where();
        where.Equal("nickname", nickname.text);
        var bro1 = Backend.GameData.Get("USER_DATA", where);

        if (bro1.IsSuccess())
        {
            var data = bro1.FlattenRows();
            if (data.Count > 0)
            {
                Debug.LogError("이미 존재하는 닉네임입니다: " + nickname.text);
                StatusText.text = "닉네임 생성에 실패했습니다. 이미존재하는 닉네임입니다.";
            }
            else
            {
                Debug.Log("닉네임이 중복되지 않습니다. 새 닉네임을 생성합니다.");
                var createBro = Backend.BMember.CreateNickname(nickname.text);

                if (createBro.IsSuccess())
                {
                    Debug.Log("닉네임 생성 성공: " + nickname.text);
                    SceneManager.LoadScene("LobbyScene");
                }
                else
                {
                    Debug.LogError("닉네임 생성 실패: " + createBro);
                    StatusText.text = "닉네임 생성에 실패했습니다. 다른 닉네임을 사용해 주세요.";
                }
            }
        }
        else
        {
            Debug.LogError("닉네임 중복 확인 실패: " + bro1);
        }
    }

    public void CheckAndCreateNickname()
    {
        // 사용자 정보를 가져옵니다.
        var bro = Backend.BMember.GetUserInfo();

        if (bro.IsSuccess())
        {
            // 닉네임이 있는지 확인합니다.
            if (bro.GetReturnValuetoJSON()["row"]["nickname"] != null)
            {
                Debug.Log("닉네임이 이미 존재합니다: " + bro.GetReturnValuetoJSON()["row"]["nickname"]);
                SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                nickname.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("사용자 정보 조회 실패: " + bro);
        }
    }

    public void Exix()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }



}
