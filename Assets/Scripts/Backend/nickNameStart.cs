using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class nickNameStart : MonoBehaviour
{
    [SerializeField] private Text nicknameText;

    private void Start()
    {
        nicknameText.text = "닉네임 :"+BackendLogin.Instance.GetNickName();
    }
}
