/**
 * @file ResultManager.cs
 * @brief 
 * @author T.Shibata
 * @date 2021/09/10 作成
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UniRx;
using UniRx.Triggers;
using TMPro;

/**
 * @class ResultManager
 * @brief 
 */
public class ResultManager : SingletonMonoBehaviour<ResultManager> {
    [SerializeField] private CinemachineVirtualCamera m_gameCinemachineVirtualCamera = default;
    [SerializeField] private CanvasGroup              m_resultCanvas                 = default;
    [SerializeField] private TextMeshProUGUI          m_pressKeyText                 = default;

    private CinemachineTransposer m_gameCinemachineTransposer;
    private bool                  m_isBackTitle;
    private bool                  m_isSelected;
    // ゲームリザルト確認イベント
    private Subject<bool> m_resultConfSubject = new Subject<bool>();
    public IObservable<bool> OnResultConfSubject {
        get { return m_resultConfSubject; }
    }

    /**
     * @brief 開始時処理
     * @return なし
     */
    void Start() {
        m_gameCinemachineTransposer = m_gameCinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        this.UpdateAsObservable()
            .Where(_ => m_isBackTitle && Input.GetKeyDown(KeyCode.Return))
            .Subscribe(_ => {
                m_gameCinemachineVirtualCamera.Priority = 9;
                m_isSelected = true;
                m_isBackTitle = false;
                m_resultConfSubject.OnNext(true);
                TitleManager.Instance.OnStart();
            }).AddTo(this);
    }

    public void OnResult() {
        m_gameCinemachineVirtualCamera.Priority = 11;

        m_resultCanvas.alpha          = 1.0f;
        m_resultCanvas.interactable   = true;
        m_resultCanvas.blocksRaycasts = true;
        m_isBackTitle                 = false;
        m_isSelected                  = false;
        StartCoroutine("resultAnimation");
    }

    private IEnumerator resultAnimation() {
        float count = 0;
        while (!m_isSelected) {
            m_pressKeyText.text = "PRESS ENTER" + new string('.', (int)(count * 3) % 3);
            count +=0.01f;
            m_gameCinemachineTransposer.m_FollowOffset.x = 15 * Mathf.Sin(count);
            m_gameCinemachineTransposer.m_FollowOffset.z = 15 * Mathf.Cos(count);
            if(count > 3 && !m_isBackTitle) {
                m_isBackTitle = true;
            }
            yield return new WaitForSeconds(0.01f);
        }        
    }
}
