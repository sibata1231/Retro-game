//  PlayFabLogin.cs
//  http://kan-kikuchi.hatenablog.com/entry/PlayFabLogin
//
//  Created by kan.kikuchi on 2019.11.04.

using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

/// <summary>
/// PlayFabのログイン処理を行うクラス
/// </summary>
public class PlayFabLogin : MonoBehaviour {

    //アカウントを作成するか
    private bool _shouldCreateAccount;

    //ログイン時に使うID
    private string _customID;

    //=================================================================================
    //ログイン処理
    //=================================================================================

    //ログイン実行
    public void Login() {
        _customID = "";
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //ログイン成功
    private void OnLoginSuccess(LoginResult result) {
        //アカウントを作成しようとしたのに、IDが既に使われていて、出来なかった場合
        if (_shouldCreateAccount && !result.NewlyCreated) {
            Debug.LogWarning($"CustomId : {_customID} は既に使われています。");
            Login();//ログインしなおし
            return;
        }

        Debug.Log($"PlayFabのログインに成功\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\nアカウントを作成したか : {result.NewlyCreated}");
    }

    //ログイン失敗
    private void OnLoginFailure(PlayFabError error) {
        Debug.LogError($"PlayFabのログインに失敗\n{error.GenerateErrorReport()}");
    }
}