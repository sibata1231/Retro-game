/**
 * @file   BlockManager.cs
 * @brief  ブロックマネージャ
 * @author T.Shibata
 * @date 2021/08/03 作成
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMain;
using UniRx;

/**
 * @class BlockManager
 * @brief ブロックマネージャクラス
 */
public class BlockManager : SingletonMonoBehaviour<BlockManager> {
    [SerializeField] private BlockController m_blockController = default; //!< ブロックコントローラー
    [SerializeField] private GameObject[]    m_blockPrefabs    = default; //!< ブロックプレハブ

    private List<Block> m_blockLists;   //!< ブロック生成リスト
    private int         m_level;        //!< レベル
    private int         m_score;        //!< スコア
    private int         m_scoreCount;   //!< スコアカウント
    private bool        m_isScoreCount; //!< スコアカウントフラグ

    // 落下イベント
    private Subject<int> m_addScoreSubject = new Subject<int>();
    public IObservable<int> OnAddScore {
        get { return m_addScoreSubject; }
    }

    // ゲーム終了判定イベント
    private Subject<bool> m_gameFinSubject = new Subject<bool>();
    public IObservable<bool> OnGameFinSubject {
        get { return m_gameFinSubject; }
    }

    private void Start() {
        m_blockController.OnFalled.Subscribe(x => {
            addBlock();
        });
        m_blockController.OnLineUp.Subscribe(x => {
            StartCoroutine("checkScoreCount");
        });
        m_blockController.OnGameFinSubject.Subscribe(x => {
            m_gameFinSubject.OnNext(true);
        });
    }

    /**
     * @brief 開始時処理
     * @return なし
     */
    public void OnStart() {
        m_level      = 0;
        m_scoreCount = 1;
        m_blockController.OnStart();

        addBlock();
    }

    private IEnumerator checkScoreCount() {
        m_scoreCount++;
        if (m_isScoreCount) {
            yield break;
        }
        m_isScoreCount = true;
        while (m_isScoreCount) {
            yield return new WaitForSeconds(0.3f);
            m_isScoreCount = false;
        }
        for (int i = 0; i < m_scoreCount; i++) {
            m_score += 100 * i;
        }
        m_scoreCount = 1;
        m_addScoreSubject.OnNext(m_score);
    }

    /**
     * @brief ブロック追加処理
     * @return なし
     */
    private void addBlock() {
        int index              = UnityEngine.Random.Range(0, m_blockPrefabs.Length);
        GameObject blockObject = Instantiate(m_blockPrefabs[index], new Vector3(0.5f, 4.0f, 0.5f), Quaternion.identity, transform);
        Block block            = blockObject.GetComponent<Block>();

        m_blockController.Register(block); // プレイヤー制御
    }    
}
