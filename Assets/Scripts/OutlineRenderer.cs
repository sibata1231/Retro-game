using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class OutlineRendererPass : ScriptableRenderPass {

    const string NAME = nameof(OutlineRendererPass);

    Material outlineMaterial = null;
    RenderTargetIdentifier renderTargetID = default;
    OutlineRenderer parentRendererFeature;

    public OutlineRendererPass(OutlineRenderer parentRendererFeature, Material outlineMaterial) {
        // ここでこのパスをどのタイミングで挿入するか決める
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        this.parentRendererFeature = parentRendererFeature;
        this.outlineMaterial = outlineMaterial;
    }

    public void SetRenderTarget(RenderTargetIdentifier renderTargetID) {
        this.renderTargetID = renderTargetID;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

        // 一見いらないように見えるが、SceneView切替時などでリセットされることがあり、ないと画面がマゼンダのアレになる
        if (parentRendererFeature.contentRenderers == null) return;

        CommandBuffer CB = CommandBufferPool.Get(NAME);
        CameraData camData = renderingData.cameraData;

        int texId = Shader.PropertyToID("_MainTex");
        int w = camData.camera.scaledPixelWidth;
        int h = camData.camera.scaledPixelHeight;
        int shaderPass = 0;

        CB.GetTemporaryRT(texId, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        CB.SetRenderTarget(texId);
        CB.ClearRenderTarget(false, true, Color.clear);

        // アウトラインを着けたいオブジェクトを描画
        foreach (Renderer renderer in GameObject.FindGameObjectsWithTag("Outline").Select(x => x.GetComponent<Renderer>()).ToList()) {
            if (renderer == null) continue; // Play終了時に、Listは破棄されずその参照先のみ破棄されるのでScene描画のために入れとかないと例のマゼンダ(ry
            // オブジェクトのマテリアルすべてでかつForwardパスで描画
            for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                CB.DrawRenderer(renderer, renderer.sharedMaterials[i], i, 0);
            }
        }

        CB.Blit(texId, renderTargetID, outlineMaterial, shaderPass);

        context.ExecuteCommandBuffer(CB);
        CommandBufferPool.Release(CB);
    }
}

public class OutlineRenderer : ScriptableRendererFeature {

    // インスペクターで設定する　アウトライン描画するマテリアル
    public Material outlineMaterial;
    // アウトラインをつけたいオブジェクトのRendererのリスト
    public List<Renderer> contentRenderers;

    OutlineRendererPass outlineRenderPass = null;

    public override void Create() {
        if (outlineRenderPass == null) {
            outlineRenderPass = new OutlineRendererPass(this, outlineMaterial);
        }

        contentRenderers = GameObject.FindGameObjectsWithTag("Outline").Select(x => x.GetComponent<Renderer>()).ToList();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        outlineRenderPass.SetRenderTarget(renderer.cameraColorTarget);
        renderer.EnqueuePass(outlineRenderPass);
    }

}