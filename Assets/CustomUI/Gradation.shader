Shader "Gradation/Gradation" {
	Properties{
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_TransparentStartY("Transparent Start Y", Range(0, 1)) = 0.75 // 透明化が始まるUVのY座標 (0が下、1が上)
		_BottomAlpha("Bottom Area Alpha", Range(0, 1)) = 0.4 // 下部エリアのアルファ値 (0.4で60%透過)
	}
	SubShader{
		Tags { 
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
		}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 100

		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _BaseColor;
			fixed _TransparentStartY;
			fixed _BottomAlpha; // 下部エリアのアルファ値

			struct appdata {
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct v2f {
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : COLOR {
				fixed4 finalColor = _BaseColor;
				
				if (i.uv.y < _TransparentStartY) {
					// UV.y が _TransparentStartY より小さい場合 (下部のエリア)
					finalColor.a = _BaseColor.a * _BottomAlpha; // 基本色のアルファに下部エリアのアルファを乗算
				} else {
					// UV.y が _TransparentStartY 以上の場合 (上部のエリア)
					// _TransparentStartY の位置で _BottomAlpha、UV.y = 1.0 の位置でアルファが0.0になるように線形補間
					fixed transitionRange = 1.0 - _TransparentStartY;
					fixed targetAlphaStart = _BaseColor.a * _BottomAlpha; // グラデーション開始時のアルファ値

					if (transitionRange <= 0.0001) { 
						if (i.uv.y >= _TransparentStartY) { 
							finalColor.a = 0.0; // ほぼ頂点なら完全に透明
						} else { 
							finalColor.a = targetAlphaStart;
						}
					} else {
						fixed progressInTransition = (i.uv.y - _TransparentStartY) / transitionRange;
						finalColor.a = lerp(targetAlphaStart, 0.0, progressInTransition);
					}
				}
				finalColor.a = clamp(finalColor.a, 0.0, 1.0);
				
				return finalColor;
			}
			ENDCG
		}
	}
}