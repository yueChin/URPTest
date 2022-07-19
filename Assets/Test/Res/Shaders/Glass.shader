Shader "Universal Render Pipeline/GlassShader"
{
    Properties
    {
		_GlassColor("玻璃颜色", Color) = (0.325, 0.807, 0.971, 0.725)
		_GlassTransparent("透明度",  Range(0.001,1))=0.1
		_HighlightColor("高光颜色", Color) = (1, 1, 1, 1)
		_HighlightPow("高光范围",  Range(0.001,1))=0.1
		_ReflectionRate("反射率",  Range(0.001,1))=0.1
		_RefractionRate("折射率",  Range(-1,1))=0.1
		_BumpMap("法线贴图",2D) = "bump"{}
		_BumpScale("法线贴图强度",Range(0,2.0)) = -1.0
        _CubeMap("环境球贴图",cube) = "Skybox"{}
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline" 
            "LightMode" = "UniversalForward"
		}

        Pass
        {
			// Transparent "normal" blending.
			Blend SrcAlpha OneMinusSrcAlpha
			//ZWrite Off

            HLSLPROGRAM
			#define SMOOTHSTEP_AA 0.1

            #pragma vertex vert
            #pragma fragment frag

			#include "Assets/Test/Res/Shaders/GlassPass.hlsl"


            ENDHLSL
        }
    }
}
