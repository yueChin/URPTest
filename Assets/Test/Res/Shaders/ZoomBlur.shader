﻿Shader "PostEffect/ZoomBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float2 _FocusScreenPosition;
            float _FocusPower;
            int _FocusDetail;
            int _ReferenceResolutionX;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenPoint = _FocusScreenPosition + _ScreenParams.xy / 2;
                float2 uv = i.uv;
                float2 mousePos = (screenPoint.xy / _ScreenParams);
                float2 focus = uv - mousePos;
                fixed aspectX = _ScreenParams.x / _ReferenceResolutionX;
                float4 outColor = float4(0,0,0,1);
                for (int i = 0; i < _FocusDetail ;i ++)
                {
                    float power = 1.0 - _FocusPower * (1.0 / _ScreenParams.x * aspectX) * float(i);
                    outColor.rgb += tex2D(_MainTex,focus * power + mousePos).rgb;
                }
                outColor.rgb *= 1.0 / float(_FocusDetail);
                return outColor;
            }
            ENDCG
        }
    }
}