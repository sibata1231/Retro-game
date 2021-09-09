/**
 * @file TitleManager.cs
 * @brief 
 * @author T.Shibata
 * @date 2021/09/09 作成
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using UniRx;

public enum GameModes {
    STANDARD = 0,
    CHALLENGE, 
    EXIT,
}

/**
 * @class TitleManager
 * @brief 
 */
public class TitleManager : SingletonMonoBehaviour<TitleManager> {
    [SerializeField] private Transform[]              m_blocks                        = default;
    [SerializeField] private CanvasGroup              m_titleCanvas                   = default;
    [SerializeField] private CinemachineVirtualCamera m_titleCinemachineVirtualCamera = default;
    [SerializeField] private GameObject               m_titleObject                   = default;
    [SerializeField] private Image                    m_backGround                    = default;

    private CinemachineTransposer              m_titleCinemachineTransposer;
    private CinemachineBasicMultiChannelPerlin m_titleCinemachineBasicMultiChannelPerlin;

    // 落下イベント
    private Subject<GameModes> m_gameStartSubject = new Subject<GameModes>();
    public IObservable<GameModes> OnGameStart {
        get { return m_gameStartSubject; }
    }
    /**
     * @brief 開始時処理
     * @return なし
     */
    void Start() {
        m_titleObject.SetActive(false);
        m_titleCinemachineTransposer              = m_titleCinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        m_titleCinemachineBasicMultiChannelPerlin = m_titleCinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_backGround.enabled = false;
        startAnimation();
    }

    /**
     * @brief 更新処理
     * @return なし
     */
    void Update() {
        
    }

    public void startAnimation() {
        foreach (var block in m_blocks) {
            block.DOMoveZ(4.5f, UnityEngine.Random.Range(0.5f, 3.0f)).SetEase(Ease.InExpo);
        }
        Vector3 followOffset = m_titleCinemachineTransposer.m_FollowOffset;
        float   gain         = m_titleCinemachineBasicMultiChannelPerlin.m_AmplitudeGain;

        DOVirtual.DelayedCall(
            4.5f,
            () => {
                m_backGround.enabled = true;
                m_titleObject.SetActive(true);
                m_titleCanvas.DOFade(1.0f, 0.3f).OnComplete(() => m_backGround.DOFade(0.0f, 0.5f).OnComplete(() => m_backGround.enabled = false));
                foreach (var block in m_blocks) {
                    block.gameObject.SetActive(false);
                }
                DOTween.To(() => followOffset, (val) => {
                    m_titleCinemachineTransposer.m_FollowOffset = val;
                }, new Vector3(0, -0.5f, -1), 1.0f).SetEase(Ease.OutBounce).OnComplete(()=> {
                });
                DOTween.To(() => gain, (val) => {
                    m_titleCinemachineBasicMultiChannelPerlin.m_AmplitudeGain = val;
                }, 0.0f, 1.0f);
            }
        );
    }

    public void endAnimation(GameModes gameModes) {
        m_titleCinemachineVirtualCamera.m_Priority = 9;
        m_titleCanvas.DOFade(0.0f, 0.3f).OnComplete(()=> m_gameStartSubject.OnNext(gameModes));
    }

    public void SendStandard() {
        endAnimation(GameModes.STANDARD);
    }

    public void SendChallenge() {
        endAnimation(GameModes.CHALLENGE);
    }


    public void SendExit() {
        m_gameStartSubject.OnNext(GameModes.EXIT);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
    }
}
