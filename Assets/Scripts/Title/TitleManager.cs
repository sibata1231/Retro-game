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
using UniRx.Triggers;

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
    [SerializeField] private AudioSource              m_audioSource                   = default;
    [SerializeField] private AudioSource              m_bgmSource                     = default;
    [SerializeField] private AudioClip                m_seBlockPush                   = default;
    [SerializeField] private AudioClip                m_seEnter                       = default;

    private CinemachineTransposer              m_titleCinemachineTransposer;
    private CinemachineBasicMultiChannelPerlin m_titleCinemachineBasicMultiChannelPerlin;
    private bool m_isStart;
    private bool m_isOnce;

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
        m_isOnce = false;
        m_titleCinemachineTransposer              = m_titleCinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        m_titleCinemachineBasicMultiChannelPerlin = m_titleCinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        OnStart();

        this.UpdateAsObservable()
            .Where(_ => !m_isStart && Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => {
                m_audioSource.PlayOneShot(m_seEnter);
                m_isStart = true;
                SendStandard();
            }).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => !m_isStart && Input.GetKeyDown(KeyCode.Escape))
            .Subscribe(_ => {
                m_audioSource.PlayOneShot(m_seEnter);
                m_isStart = true;
                SendExit();
            }).AddTo(this);
    }
    
    public void OnStart() {
        m_bgmSource.DOFade(0.5f, 1.0f);
        m_bgmSource.Play();

        m_isStart = true;
        m_titleObject.SetActive(false);
        m_backGround.enabled = false;
        m_titleCinemachineVirtualCamera.m_Priority = 11;
        startAnimation();
    }

    public void startAnimation() {
        if (!m_isOnce) {
            foreach (var block in m_blocks) {
                block.DOMoveZ(4.5f, UnityEngine.Random.Range(0.5f, 3.0f)).SetEase(Ease.InExpo).OnComplete(() => m_audioSource.PlayOneShot(m_seBlockPush));
            }
            m_isOnce = true;
        }
        Vector3 followOffset = m_titleCinemachineTransposer.m_FollowOffset;
        float   gain         = m_titleCinemachineBasicMultiChannelPerlin.m_AmplitudeGain;

        DOVirtual.DelayedCall(
            4.0f,
            () => {
                m_backGround.enabled = true;
                m_isStart            = false;
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
        m_bgmSource.DOFade(0.0f, 1.0f).OnComplete(() => m_bgmSource.Stop());
        m_titleObject.SetActive(false);
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
