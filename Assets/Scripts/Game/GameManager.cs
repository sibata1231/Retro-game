/**
 * @file GameManager.cs
 * @brief 
 * @author T.Shibata
 * @date 2021/09/09 作成
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UniRx;

/**
 * @class GameManager
 * @brief 
 */
public class GameManager : SingletonMonoBehaviour<GameManager> {
    [SerializeField] private CinemachineVirtualCamera m_gameCinemachineVirtualCamera = default;

    /**
     * @brief 開始時処理
     * @return なし
     */
    private void Start() {
        TitleManager.Instance.OnGameStart.Subscribe(gameMode => {
            switch (gameMode) {
                case GameModes.STANDARD:
                    BlockManager.Instance.OnStart();
                    break;
                case GameModes.CHALLENGE:
                    BlockManager.Instance.OnStart();
                    break;
            }
        });
    }
}
