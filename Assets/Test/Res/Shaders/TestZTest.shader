Shader "Universal Render Pipeline/TestShaderTerms"
{
    Properties
    {
        [MainColor]   _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("BaseMap", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Tansparent" 
            "IgnoreProjector" = "True" 
            "RenderPipeline" = "UniversalPipeline" 
            "LightMode" = "UniversalForward"
        }
        AlphaToMask Off 
        //On Off 
        
        ColorMask B
        //R G B 0
        
        Blend One OneMinusDstColor //Test Blend ----- 
        //SrcAlpha OneMinusSrcAlpha
        //One One
        //OneMinusDstColor One
        //DstColor Zero
        //DstColor SrcColor
        //SrcColor One
        //OneMinusSrcColor One
        //Zero OneMinusSrcColor
        
        ZWrite On //Test Z -----
        //On Off
            
        ZTest Less
        //Less Greater LEqual GEqual Equal NotEqual Always
                
        Cull Off //Test Cull 
        //-----Off Back Front

        Pass
        {
            Name "JustTest"

            HLSLPROGRAM

            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5
            
            #pragma vertex vert
            #pragma fragment frag

            // -------------------------------------

            float _AlphaClip;
            float4 _Color;

            #include "Packages/com.unity.render-pipelines.universal@10.9.0/Shaders/UnlitInput.hlsl"
            
            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                // float fogCoord  : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.uv = half2(0,0);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
