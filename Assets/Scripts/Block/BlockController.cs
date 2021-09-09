/**
 * @file BlockController.cs
 * @brief 
 * @author T.Shibata
 * @date 2021/09/02 作成
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using GameMain;

/**
 * @class BlockController
 * @brief 
 */
public class BlockController : MonoBehaviour {
    // 落下イベント
    private Subject<bool> m_fallSubject = new Subject<bool>();
    public IObservable<bool> OnFalled {
        get { return m_fallSubject; }
    }
    // 落下イベント
    private Subject<bool> m_isLineUpSubject = new Subject<bool>();
    public IObservable<bool> OnLineUp {
        get { return m_isLineUpSubject; }
    }

    private Block m_currentBlock; //!< ブロック
    private bool  m_isActive;     //!< 制御できるかどうか
    public bool IsActive {
        get => m_isActive;
    }

    /**
     * @brief 開始時処理
     * @return なし
     */
    public void OnStart() {
        // 初期化処理
        m_isActive = true;

        // 回転処理
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.UpArrow) && m_currentBlock.IsRotate(new Vector3(90, 0, 0)))
            .Subscribe(_ => m_currentBlock?.Rotate(new Vector3(90, 0, 0))).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.DownArrow) && m_currentBlock.IsRotate(new Vector3(-90, 0, 0)))
            .Subscribe(_ => m_currentBlock?.Rotate(new Vector3(-90, 0, 0))).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.LeftArrow) && m_currentBlock.IsRotate(new Vector3(0, 0, -90)))
            .Subscribe(_ => m_currentBlock?.Rotate(new Vector3(0, 0, -90))).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.RightArrow) && m_currentBlock.IsRotate(new Vector3(0, 0, 90)))
            .Subscribe(_ => m_currentBlock?.Rotate(new Vector3(0, 0, 90))).AddTo(this);

        // 移動処理
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.W) && m_currentBlock.IsMove(1.0f,Axistypes.Z))
            .Subscribe(_ => m_currentBlock?.Move(1.0f, Axistypes.Z)).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.S) && m_currentBlock.IsMove(-1.0f, Axistypes.Z))
            .Subscribe(_ => m_currentBlock?.Move(-1.0f, Axistypes.Z)).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.A) && m_currentBlock.IsMove(-1.0f, Axistypes.X))
            .Subscribe(_ => m_currentBlock?.Move(-1.0f, Axistypes.X)).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.D) && m_currentBlock.IsMove(1.0f, Axistypes.X))
            .Subscribe(_ => m_currentBlock?.Move(1.0f, Axistypes.X)).AddTo(this);

        // 落下処理
        this.UpdateAsObservable()
            .Where(_ => IsActive && m_currentBlock.IsActive)
            .Subscribe(_ => m_currentBlock.FallMove(Time.deltaTime * 0.1f)).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => IsActive && Input.GetKeyDown(KeyCode.Z))
            .Subscribe(_ => {
                m_currentBlock.Fall();
            }).AddTo(this);
    }

    /**
     * @brief ブロック登録処理
     * @param[in] block 
     * @return なし
     */
    public void Register(Block block) {
        m_currentBlock = block;
        m_currentBlock.OnActive.Subscribe(_ => {
            m_fallSubject.OnNext(true);
        });
        m_currentBlock.OnLineUpSubject.Subscribe(_ => {
            m_isLineUpSubject.OnNext(true);
        });
    }
}
