using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class LoginPun : MonoBehaviourPunCallbacks {
       // ログイン処理
    public void Login() {
        PhotonNetwork.ConnectUsingSettings();
    }

    //ルームに入室前に呼び出される
    public override void OnConnectedToMaster() {
        Debug.Log("ログイン〇");
    }
}
