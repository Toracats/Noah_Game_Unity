Shader "UI/TopFadeGradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _FadeTopPercent ("Fade Top Percent (0-1)", Range(0.0, 1.0)) = 0.2

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc" // UnityUI.cginc は引き続き必要です

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1; // RectMask2Dクリッピング用に元の頂点位置を保持
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _MainTex_ST;
            float4 _ClipRect; // RectMask2D 用

            float _FadeTopPercent;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                // RectMask2Dのクリッピングのためにワールド座標(実際にはローカル座標のまま)を渡す
                // Canvasのレンダリングでは、頂点座標はすでにCanvas空間に変換されていることが多い
                OUT.worldPosition = v.vertex; 
                
                // ここを変更: UnityObjectToClipPos を使用
                OUT.vertex = UnityObjectToClipPos(v.vertex);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 TColor = tex2D(_MainTex, IN.texcoord);
                half4 finalColor = TColor * IN.color;

                float fade_start_y = 1.0 - _FadeTopPercent;
                float fade_factor = 1.0;
                
                if (_FadeTopPercent > 0.001) 
                {
                    fade_factor = 1.0 - smoothstep(fade_start_y, 1.0, IN.texcoord.y);
                }
                
                finalColor.a *= fade_factor;

                #ifdef UNITY_UI_CLIP_RECT
                // UnityGet2DClipping は canvas space の座標を期待するので、
                // vert シェーダーで渡した worldPosition (実質ローカル/キャンバス空間座標) を使用
                finalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (finalColor.a - 0.001);
                #endif

                return finalColor;
            }
        ENDCG
        }
    }
}