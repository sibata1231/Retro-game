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
using TMPro;

/**
 * @class GameManager
 * @brief 
 */
public class GameManager : SingletonMonoBehaviour<GameManager> {
    [SerializeField] private CinemachineVirtualCamera m_gameCinemachineVirtualCamera = default;
    [SerializeField] private CanvasGroup              m_gameCanvas                   = default; 
    [SerializeField] private TextMeshProUGUI          m_scoreText                    = default;
    [SerializeField] private GameObject[]             m_gameObjects                  = default;

    /**
     * @brief 開始時処理
     * @return なし
     */
    private void Start() {
        TitleManager.Instance.OnGameStart.Subscribe(gameMode => {
            switch (gameMode) {
                case GameModes.STANDARD:
                    gameStartAnimation();
                    BlockManager.Instance.OnStart();
                    break;
                case GameModes.CHALLENGE:
                    gameStartAnimation();
                    BlockManager.Instance.OnStart();
                    break;
            }
        });

        BlockManager.Instance.OnAddScore.Subscribe(score => {
            m_scoreText.text = score.ToString();
        });

        BlockManager.Instance.OnGameFinSubject.Subscribe(x => {
            m_gameCinemachineVirtualCamera.Priority = 9;
            foreach (var obj in m_gameObjects) {
                obj.SetActive(false);
            }

            m_gameCanvas.alpha          = 1.0f;
            m_gameCanvas.interactable   = false;
            m_gameCanvas.blocksRaycasts = false;
            ResultManager.Instance.OnResult();
        });

        ResultManager.Instance.OnResultConfSubject.Subscribe(x => {
            m_gameCanvas.alpha = 0.0f;
            foreach (var obj in m_gameObjects) {
                obj.SetActive(true);
            }
            foreach (var obj in GameObject.FindGameObjectsWithTag("Block")) {
                Destroy(obj);
            }
        });
    }

    private void gameStartAnimation() {
        m_gameCinemachineVirtualCamera.Priority = 11;
        m_gameCanvas.alpha          = 1.0f;
        m_gameCanvas.interactable   = true;
        m_gameCanvas.blocksRaycasts = true;
    }
}
