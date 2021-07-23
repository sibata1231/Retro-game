/**
 * @file WireFrameModel.cs
 * @brief 
 * @author T.Shibata
 * @date 2021/07/23 作成
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class WireFrameModel
 * @brief 
 */
public class WireFrameModel : MonoBehaviour {

    /**
     * @brief 開始時処理
     * @return なし
     */
    void Start() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);
    }
}
