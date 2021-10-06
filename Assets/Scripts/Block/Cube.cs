/**
 * @file Cube.cs
 * @brief 
 * @author T.Shibata
 * @date 2021/09/02 作成
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;

namespace GameMain {

    public enum BlockColor {
        BLUE = 0,
        GREEN,
        CYAN,
        RED,
        MAGENTA,
        ORANGE,
        WHITE,
        MAX,
    }

    /**
     * @class Cube
     * @brief キューブクラス
     */
    public class Cube : MonoBehaviour {
        [Header("===== Main =====")]
        [SerializeField] private Renderer   m_mainRenderer     = default; //!< メイン(Main)
        [SerializeField] private Material   m_mainMaterial     = default; //!< メイン(Main)
        [SerializeField] private Material   m_dissolveMaterial = default; //!< 削除時エフェクト

        [Header("===== Points =====")]
        [SerializeField] private Renderer[] m_subRenderers  = default; //!< サブ(Points)
        [SerializeField] private Material   m_pointMaterial = default; //!< サブ(Points)

        [Header("===== Planes =====")]
        [SerializeField] private Transform[] m_planes = default; //!< サブ(Plane)

        [Header("===== Effects =====")]
        [SerializeField] private ParticleSystem m_cubeEffect = default; //!< エフェクト

        private const string MAIN_BASE_COLOR_NAME      = "_BaseColor";     //!< プロパティ変更名称
        private const string MAIN_EMISSION_COLOR_NAME  = "_EmissionColor"; //!< プロパティ変更名称
        private const string POINT_EMISSION_COLOR_NAME = "_EmissionColor"; //!< プロパティ変更名称
        private const string DISSOLVE_COLOR_NAME       = "_EageColor";     //!< プロパティ変更名称
        private const string DISSOLVE_POWER_NAME       = "_Power";         //!< プロパティ変更名称

        private Material m_main;
        private Material m_point;
        private Material m_dissolve;
        private int      m_mainColorPropertyId;    //!< MainプロパティID
        private int      m_mainEmissionPropertyId; //!< MainプロパティID
        private int      m_dissoveColorPropertyId; //!< DissolveプロパティID
        private int      m_dissovePowerPropertyId; //!< DissolveプロパティID
        private int      m_pointPropertyId;        //!< SubプロパティID
        private bool     m_isActive;

        public bool IsActive {
            get => m_isActive;  
        }
        public int ColorToInt(float height) =>  (4 - (int) Math.Abs(height)) % (int) BlockColor.MAX;

        /**
         * @brief 開始時処理
         * @return なし
         */
        private void Start() {
            m_main     = new Material(m_mainMaterial);
            m_point    = new Material(m_pointMaterial);
            m_dissolve = new Material(m_dissolveMaterial);

            m_mainRenderer.material  = m_main;
            foreach(var renderer in m_subRenderers) {
                renderer.material = m_point;
            }
            m_mainColorPropertyId    = Shader.PropertyToID(MAIN_BASE_COLOR_NAME);
            m_mainEmissionPropertyId = Shader.PropertyToID(MAIN_EMISSION_COLOR_NAME);
            m_pointPropertyId        = Shader.PropertyToID(POINT_EMISSION_COLOR_NAME);
            m_dissoveColorPropertyId = Shader.PropertyToID(DISSOLVE_COLOR_NAME);
            m_dissovePowerPropertyId = Shader.PropertyToID(DISSOLVE_POWER_NAME);
            m_main.SetColor(m_mainColorPropertyId,    Color.cyan);
            m_main.SetColor(m_mainEmissionPropertyId, Color.cyan);
            m_point.SetColor(m_pointPropertyId,       Color.cyan);
        }

        /**
         * @brief 作成時処理
         * @return なし
         */
        public void Create() {
            m_isActive = true;
        }

        /**
         * @brief  削除処理
         * @return なし
         */
        public void Delete(float height) {
            m_isActive = false;
            setColor((BlockColor)(ColorToInt(height)));
        }

        /**
         * @brief  削除処理
         * @return なし
         */
        public void Destroy() {
            m_mainRenderer.material = m_dissolve;
            m_dissolve.SetColor(m_dissoveColorPropertyId, m_main.GetColor(m_mainColorPropertyId));
            DOTween.To(() => 0.0f, (x) => m_dissolve.SetFloat(m_dissovePowerPropertyId, x), 1.0f, 0.3f)
                   .OnUpdate(() => Debug.Log(m_dissolve.GetFloat(m_dissovePowerPropertyId).ToString()))
                   .OnComplete(() => Destroy(this.gameObject));
        }

        /**
         * @brief 
         * @return なし
         */
        private void setColor(BlockColor blockColor) {
            foreach(var plane in m_planes) {
                plane.gameObject.SetActive(false);
            }
            //Debug.Log(blockColor);
            Color color = Color.black;
            switch (blockColor) {
                case BlockColor.BLUE:    color = Color.blue;        break;
                case BlockColor.MAGENTA: color = Color.magenta;     break;
                case BlockColor.GREEN:   color = Color.green;       break;
                case BlockColor.CYAN:    color = Color.cyan;        break;
                case BlockColor.ORANGE:  color = Color.yellow*0.5f; break;
                case BlockColor.RED:     color = Color.red;         break;
                case BlockColor.WHITE:   color = Color.white;       break;
            }
            m_main.SetColor(m_mainColorPropertyId, color);
            m_main.SetColor(m_mainEmissionPropertyId, color);
            m_point.SetColor(m_pointPropertyId, color);
        }
    }
}
