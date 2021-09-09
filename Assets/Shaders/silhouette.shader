Shader "Unlit/silhouette" {
    Properties{
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Range(0, 10)) = 1
        _Cutoff("Cutoff Level", Range(0, 1)) = 0
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
    }
    SubShader{
        Tags { "RenderType" = "Overlay" "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZTest Always

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half2 _MainTex_TexelSize;

            half4 _OutlineColor;
            half _OutlineWidth;

            half _Cutoff;

            float4 _EmissionColor;             //追加

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings {
                float2 uv         : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input) {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings i) : SV_Target {

                half2 uv = i.uv;
                half2 destUV = _MainTex_TexelSize * _OutlineWidth;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                half sum = 0;

                [unroll(3)]
                for (int i = -1; i <= 1; i++) {
                    [unroll(3)]
                    for (int j = -1; j <= 1; j++) {
                        sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(destUV.x * i, destUV.y * j)).a;
                    }
                }

                sum = saturate(sum);
                clip(_Cutoff - col.a);

                half4 outline = sum * _OutlineColor;

                return outline * _EmissionColor;
            }
            ENDHLSL
        }
    }
}