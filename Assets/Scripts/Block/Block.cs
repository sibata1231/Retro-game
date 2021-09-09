/**
 * @file Blcok.cs
 * @brief 
 * @author T.Shibata
 * @date 2021/08/03 作成
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;

namespace GameMain {

    public struct StageInfo {
        public const int WIDTH  = 10;
        public const int HEIGHT = 10;
        public const int DEPTH  = 10;

        public int m_currentWidth;
        public int m_currentHeight;
    }

    public enum Axistypes {
        X = 0,
        Y,
        Z,
    }

    /**
     * @class Block
     * @brief ブロッククラス
     */
    public class Block : MonoBehaviour{
        // 起動しているかどうか
        private bool m_isActive;
        public bool IsActive {
            get { return m_isActive; }
        }

        // 落下イベント
        private Subject<bool> m_isActiveSubject = new Subject<bool>();
        public IObservable<bool> OnActive {
            get { return m_isActiveSubject; }
        }

        // 落下イベント
        private Subject<bool> m_isLineUpSubject = new Subject<bool>();
        public IObservable<bool> OnLineUpSubject {
            get { return m_isLineUpSubject; }
        }

        private const int BLOCK_SIZE_X = 3; //!< ブロック許容値_X軸
        private const int BLOCK_SIZE_Y = 3; //!< ブロック許容値_Y軸
        private const int BLOCK_SIZE_Z = 3; //!< ブロック許容値_Z軸

        private Transform  m_fallTransform; //!<
        private List<Cube> m_cubeDatas;     //!< キューブインデックスデータ
        private bool       m_checkFall;     //!< 落下チェック

        /**
         * @brief 開始処理
         * @return なし
         */
        public void Start() {
            m_checkFall = false;
            m_isActive  = true;
            m_cubeDatas = new List<Cube>();
            m_cubeDatas.Clear();
            foreach (var data in gameObject.GetComponentsInChildren<Transform>()) {
                if (data.name == transform.name) {
                    continue;
                }
                var cube = data.GetComponent<Cube>();
                if(cube != null) {
                    cube.Create();
                    m_cubeDatas.Add(cube);
                }
            }
            m_fallTransform = null;
        }

        /**
         * @brief デバッグ描画処理
         * @return なし
         */
        private void OnDrawGizmos() {
            //RaycastHit hit;
            //foreach (var cube in m_cubeDatas) {
            //    Gizmos.DrawWireCube(cube.transform.position, Vector3.one);
            //    if (Physics.Raycast(cube.transform.position, new Vector3(1, 0, 0).normalized, out hit, 1.0f)) {
            //        if (hit.transform.gameObject.GetComponent<Cube>() == null || !hit.transform.gameObject.GetComponent<Cube>().IsActive) {
            //            Debug.DrawRay(cube.transform.position, new Vector3(1, 0, 0).normalized, Color.red);
            //        }
            //    }
            //    if (Physics.Raycast(cube.transform.position, new Vector3(-1, 0, 0).normalized, out hit, 1.0f)) {
            //        if (hit.transform.gameObject.GetComponent<Cube>() == null || !hit.transform.gameObject.GetComponent<Cube>().IsActive) {
            //            Debug.DrawRay(cube.transform.position, new Vector3(-1, 0, 0).normalized, Color.magenta);
            //        }
            //
            //    }
            //    if (Physics.Raycast(cube.transform.position, new Vector3(0, 0, 1).normalized, out hit, 1.0f)) {
            //        if (hit.transform.gameObject.GetComponent<Cube>() == null || !hit.transform.gameObject.GetComponent<Cube>().IsActive) {
            //            Debug.DrawRay(cube.transform.position, new Vector3(0, 0, 1).normalized, Color.blue);
            //        }
            //
            //    }
            //    if (Physics.Raycast(cube.transform.position, new Vector3(0, 0, -1).normalized, out hit, 1.0f)) {
            //        if (hit.transform.gameObject.GetComponent<Cube>() == null || hit.transform.gameObject.GetComponent<Cube>().IsActive) {
            //            Debug.DrawRay(cube.transform.position, new Vector3(0, 0, -1).normalized, Color.cyan);
            //        }
            //    }
            //    //Debug.DrawRay(cube.transform.position, Vector3.Scale(cube.transform.position, new Vector3( 1, 0,  0)).normalized, Color.red);
            //    //Debug.DrawRay(cube.transform.position, Vector3.Scale(cube.transform.position, new Vector3(-1, 0,  0)).normalized, Color.magenta);
            //    //Debug.DrawRay(cube.transform.position, Vector3.Scale(cube.transform.position, new Vector3( 0, 0,  1)).normalized, Color.blue);
            //    //Debug.DrawRay(cube.transform.position, Vector3.Scale(cube.transform.position, new Vector3( 0, 0, -1)).normalized, Color.cyan);     
            //}
        }

        /**
         * @brief 落下移動処理
         * @return なし
         */
        public void FallMove(float move) {
            if (!IsActive) {
                return;
            }
            RaycastHit hit;
            foreach (var cube in m_cubeDatas) {
                if (Physics.Raycast(cube.transform.position, new Vector3(0, -1, 0).normalized, out hit, 0.5f + 0.1f)) {
                    if (hit.transform.gameObject.GetComponent<Cube>() != null && hit.transform.gameObject.GetComponent<Cube>().IsActive) {
                        continue;
                    }
                    //Debug.Log(hit.collider.gameObject.name);
                    if (m_fallTransform == null) {
                        m_fallTransform = hit.transform;
                        continue;
                    }
                    if (m_fallTransform.position.y < hit.transform.position.y) {
                        m_fallTransform = hit.transform;
                    }
                }
            }
            if (m_fallTransform == null) {
                transform.position -= new Vector3(0, move, 0);
            } else {
                transform.DOLocalMoveY(m_fallTransform.position.y + 1.0f, 0.3f).OnComplete(() => checkColumnBlock());
                StartCoroutine("DeleteCoroutine");
            }
        }

        /**
         * @brief 落下移動処理
         * @return なし
         */
        public void Fall() {
            RaycastHit hit;
            foreach (var cube in m_cubeDatas) {
                if (Physics.Raycast(cube.transform.position, new Vector3(0, -1, 0).normalized, out hit, GameMain.StageInfo.DEPTH)) {
                    //Debug.Log(hit.collider.gameObject.name);
                    if (hit.transform.gameObject.GetComponent<Cube>() != null && hit.transform.gameObject.GetComponent<Cube>().IsActive) {
                        continue;
                    }
                    if (m_fallTransform == null) {
                        m_fallTransform = hit.transform;
                        continue;
                    }
                    if (m_fallTransform.position.y < hit.transform.position.y) {
                        m_fallTransform = hit.transform;
                    }
                }
            }
            transform.DOLocalMoveY(m_fallTransform.position.y + 1.0f, 0.3f).OnComplete(() => checkColumnBlock());
            StartCoroutine("DeleteCoroutine");
        }

        /**
         * @brief 削除コルーチン処理
         * @return なし
         */
        IEnumerator DeleteCoroutine() {
            m_isActive = false;
            m_isActiveSubject.OnNext(false);
            var time = 10;
            while (time > 0) {
                time--;
                yield return new WaitForSeconds(0.1f);
            }
            foreach (var cube in m_cubeDatas) {
                cube.Delete();
            }
        }

        /**
         * @brief 列チェック処理
         * @return なし
         */
        private void checkColumnBlock() {
            foreach (var cube in m_cubeDatas) {
                checkBlockArrayCount(cube.transform.position, Vector3.right,   StageInfo.WIDTH);  // 横
                checkBlockArrayCount(cube.transform.position, Vector3.forward, StageInfo.HEIGHT); // 縦
            }
        }

        private void checkBlockArrayCount(Vector3 position, Vector3 direction,float length) {
            List<GameObject> cubeDatas = new List<GameObject>();
            foreach (var hit in Physics.RaycastAll(position + direction * -length, direction, length * 2.0f)) {
                if (hit.transform.gameObject.GetComponent<Cube>()) {
                    cubeDatas.Add(hit.transform.gameObject);
                }
            }
            if (cubeDatas.Count == StageInfo.WIDTH) {
                m_isLineUpSubject.OnNext(true);
                foreach (var cube in cubeDatas) {
                    Destroy(cube);
                }
            }
        }

        /**
         * @brief 移動チェック処理
         * @return なし
         */
        public bool IsMove(float move, Axistypes axistype) {
            RaycastHit hit;
            Vector3 moveValue = new Vector3(axistype == Axistypes.X ? move : 0.0f, 0.0f, axistype == Axistypes.X ? 0.0f : move);
            foreach (var cube in m_cubeDatas) {
                var calc = cube.transform.position + moveValue;
                if      (calc.x >=  5.0f) return false;
                else if (calc.x <= -5.0f) return false;
                else if (calc.z >=  5.0f) return false;
                else if (calc.z <= -5.0f) return false;
                if (Physics.Raycast(cube.transform.position, moveValue.normalized, out hit, 0.5f)) {
                    if (hit.transform.gameObject.GetComponent<Cube>() == null || !hit.transform.gameObject.GetComponent<Cube>().IsActive) {
                        //Debug.Log(cube.gameObject.name + ":" + hit.transform.gameObject.name);
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * @brief 移動処理
         * @return なし
         */
        public void Move(float move, Axistypes axistype) {
            switch (axistype) {
                case Axistypes.X:
                    transform.DOLocalMoveX(move, 0.1f).SetRelative();
                    break;
                case Axistypes.Z:
                    transform.DOLocalMoveZ(move, 0.1f).SetRelative();
                    break;
            }
        }

        /**
         * @brief  回転チェック処理
         * @return なし
         */
        public bool IsRotate(Vector3 rotate) {
            foreach (var cube in m_cubeDatas) {
                Vector3 calc = Quaternion.Euler(rotate) * cube.transform.position;
                //Debug.Log(calc.ToString());
                if      (calc.x >=  5.0f) return false;
                else if (calc.x <= -5.0f) return false;
                else if (calc.z >=  5.0f) return false;
                else if (calc.z <= -5.0f) return false;
            }
            return true;
        }

        /**
         * @brief 回転処理
         * @return なし
         */
        public void Rotate(Vector3 rotate) {
            transform.DOLocalRotate(rotate, 0.05f)
                     .SetRelative();           
        }

        /**
         * @brief 破棄処理
         * @return なし
         */
        private void OnDestroy() {
            m_cubeDatas.Clear();
        }
    }
}
