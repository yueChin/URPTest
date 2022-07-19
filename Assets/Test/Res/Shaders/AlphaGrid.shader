// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/AlphaGrid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha ("Alpha", Range(0,1)) = 0.1
		_AlphaGridTex ("Alpha Grid Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION; 
				float2 uv : TEXCOORD0;
				float4 screenUv : TEXCOORD1;
				UNITY_FOG_COORDS(1)
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AlphaGridTex;
			//float4 _AlphaGridTex_ST;
			float _Alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(o.vertex);
				screenPos.xy *= _ScreenParams.xy / 8;//此处不能先除w，会导致插值精度不够
				o.screenUv = screenPos;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float gridAlpha = tex2Dproj(_AlphaGridTex, i.screenUv).a;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				clip(_Alpha - gridAlpha);
				return col;
			}
			ENDCG
		}
	}
}
