#ifndef UNIVERSAL_My_GLASS_INCLUDED
#define UNIVERSAL_My_GLASS_INCLUDED
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.universal@10.9.0/ShaderLibrary/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Assets/Test/Res/Shaders/GrabInput.hlsl"

//#include "UnityCG.cginc"

#define UNITY_PROJ_COORD(a) a.xyzw/a.w

struct Attributes
{
    float3 positionOS : POSITION;
    float4 uv : TEXCOORD0;
    half3 normalOS : NORMAL;
    float4 tangentOS: TANGENT;
};

struct Varyings
{
    float4 positionCS:SV_POSITION;
    float2 uv : TEXCOORD0;
    half3 normalWS:TEXCOORD1;
    half3 positionWS:TEXCOORD2;
    float4 projPos:TEXCOORD3;
    
    //切线空间转世界空间数据3代4
    float4 T2W0:TEXCOORD4;
    float4 T2W1:TEXCOORD5;
    float4 T2W2:TEXCOORD6;

    float3	TtoV0 : TEXCOORD7;
    float3	TtoV1 : TEXCOORD8;
    float3	TtoV2 : TEXCOORD9;

};

float _GlassTransparent;
float _HighlightPow;
float4 _GlassColor;
float _WaterDownRef;
float _RefractionRate;
float _ReflectionRate;

//场景预渲染
sampler2D _GrabTexture;
//定义法线贴图变量
sampler2D _BumpMap;
float4 _BumpMap_ST;
//定义法线强度变量
float _BumpScale;
float4 _HighlightColor;
//获取环境球贴图
samplerCUBE _CubeMap;

Varyings vert (Attributes v)
{
    Varyings o;

    o.positionCS = TransformObjectToHClip(v.positionOS);
    o.projPos = ComputeScreenPos(o.positionCS);
    o.projPos.z = -mul(UNITY_MATRIX_MV,v.positionOS).z;
    o.positionWS = TransformObjectToWorld(v.positionOS);
    o.normalWS = TransformObjectToWorldNormal(v.normalOS);
    
    o.uv = v.uv;

    //世界法线
    float3 worldNormal = o.normalWS;
    //世界切线
    float3 worldTangent = TransformObjectToWorld(v.tangentOS.xyz);
    //世界副切线
    float3 worldBinormal = cross(worldNormal,worldTangent)*v.tangentOS.w;
    //世界坐标
    float3 worldPos =  o.positionWS;

    //构建变换矩阵
    //z轴是法线方向(n)，x轴是切线方向(t)，y轴可由法线和切线叉积得到，也称为副切线（bitangent, b）
    o.T2W0 = float4(worldTangent.x,worldBinormal.x,worldNormal.x, worldPos.x);
    o.T2W1 = float4(worldTangent.y,worldBinormal.y,worldNormal.y, worldPos.y);
    o.T2W2 = float4(worldTangent.z,worldBinormal.z,worldNormal.z, worldPos.z);

    //获取模型到切线转换变量  rotation

    float3 binormal = cross( normalize(v.normalOS), normalize(v.tangentOS.xyz) ) * v.tangentOS.w; \
    float3x3 rotation = float3x3( v.tangentOS.xyz, binormal, v.normalOS );
    
    //转递切线到视角的转换变量组
    o.TtoV0 = normalize(mul(rotation, UNITY_MATRIX_IT_MV[0].xyz));
    o.TtoV1 = normalize(mul(rotation, UNITY_MATRIX_IT_MV[1].xyz));
    o.TtoV2 = normalize(mul(rotation, UNITY_MATRIX_IT_MV[2].xyz));
    return o;
}

float4 frag (Varyings i) : SV_Target
{

    float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz- i.positionWS.xyz);
    Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.positionWS));
    //世界坐标灯光方向
    float3 mainLightDir = normalize(mainLight.direction);
    
    //获取法线贴图
    float4 Normaltex = tex2D(_BumpMap, float2(i.uv.x*_BumpMap_ST.x+_BumpMap_ST.z,i.uv.y*_BumpMap_ST.y+_BumpMap_ST.w));
    //法线贴图0~1转-1~1
    float3 tangentNormal = UnpackNormal(Normaltex);
    //乘以凹凸系数
    tangentNormal.xy *= _BumpScale;
    //向量点乘自身算出x2+y2，再求出z的值
    tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
    //向量变换只需要3*3
    float3x3 T2WMatrix = float3x3(i.T2W0.xyz,i.T2W1.xyz,i.T2W2.xyz);
    //法线从切线空间到世界空间
    float3 worldNormal = mul(T2WMatrix,tangentNormal);

    worldNormal = normalize(worldNormal);


    //切线空间转视角空间 向量变换只需要3*3
    float3x3 TtoVMatrix = float3x3(i.TtoV0.xyz,i.TtoV1.xyz,i.TtoV2.xyz);
    //法线从切线空间到视角空间
    float3 _viewNormal = mul(TtoVMatrix,tangentNormal);
    //获取顶点世界坐标
    float3 WordPos = float3(i.T2W0.w, i.T2W1.w, i.T2W2.w);

    //摄像机视角方向
    half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - WordPos);

    //反射光线
    half3 reflectLightDir = normalize(reflect(-mainLightDir, worldNormal));

    //反射视线
    half3 reflectViewDir = normalize(reflect(-viewDir, worldNormal));		

    //反射光线与视角点乘,获得高光渐变。
    half Ramp_Specular =saturate(dot(reflectLightDir,viewDir));

    //用次方，改变高光渐变范围
    Ramp_Specular = pow(Ramp_Specular,500*_HighlightPow);

    //面比率
    half Ramp_FaceRatio =saturate( dot(worldNormal,viewDir));

    //屏幕像素转UV 0~1
    float2 screenPos = UNITY_PROJ_COORD(i.projPos);//除以w分量
    //
    //面比率控制折射扭曲强度
    _RefractionRate = Ramp_FaceRatio*_RefractionRate*2;

    //折射图案 = 预渲染图像，屏幕坐标+视角法线扭曲*扭曲强度
    half4 _refractionTex = tex2D( _GrabTexture,screenPos+_viewNormal.xy*_RefractionRate);  

    //轮廓光
    float4 OutLineColor =(1-Ramp_FaceRatio)*_HighlightColor;

    //获取颜色和透明
    float4 OutColor = _GlassColor;

    //中心与边缘颜色不同 
    _GlassColor.rgb =pow(_GlassColor.rgb,pow((1-Ramp_FaceRatio)*2,5));

    //颜色与折射图案依据面比率和透明度混色。累加反射
    OutColor.rgb = lerp(float3(0,0,0),_refractionTex*_GlassColor.rgb,pow(Ramp_FaceRatio,1/(_GlassTransparent*1)));

    //读取环境球
    float3 _cubeTex = texCUBElod(_CubeMap, half4(reflectViewDir, 0))*_ReflectionRate;
    //合成反射纹理用高光着色
    OutColor.rgb +=_cubeTex*(1-Ramp_FaceRatio)*_HighlightColor;
    //添加高光与轮廓边
    OutColor+=Ramp_Specular*_HighlightColor*_HighlightColor.a+OutLineColor*_ReflectionRate;

    OutColor.a = lerp(OutColor.a ,1,_GlassColor.a);
    return OutColor;
}
#endif
