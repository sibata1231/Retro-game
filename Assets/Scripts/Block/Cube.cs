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
    /**
     * @class Cube
     * @brief キューブクラス
     */
    public class Cube : MonoBehaviour{
        private bool m_isActive;
        public bool IsActive {
            get => m_isActive;  
        }

        public void Create() {
            m_isActive = true;
        }

        public void Delete() {
            m_isActive = false;
        }
    }
}
