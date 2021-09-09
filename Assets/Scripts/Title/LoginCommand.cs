using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginCommand : MonoBehaviour {
    [Header("========= Main =========")]
    [SerializeField] private LoginPun        m_loginPun       = default;
    [SerializeField] private TMP_InputField m_nameInputField = default;
    [Header("========= Debug =========")]
    [SerializeField] private TextMeshProUGUI m_loginText      = default;
    [SerializeField] private TextMeshProUGUI m_userNameText   = default;


    // ログイン処理
    public void OnLogin() {
        PhotonNetwork.NickName = m_nameInputField.text;
        m_loginPun.Login();
    }

    // 確認用
    private void Update() {
        m_loginText.text    = "NetworkStatus : " + PhotonNetwork.NetworkClientState.ToString();
        m_userNameText.text = "UserName : "      + PhotonNetwork.NickName;
    }

}
