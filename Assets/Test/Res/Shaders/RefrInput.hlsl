#ifndef UNIVERSAL_MY_REFR_INCLUDED
#define UNIVERSAL_MY_REFR_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

//CBUFFER_START(UnityPerMaterial)
half4 _RefractColor;
half _RefractAmount;
half _RefractRatio;
//CBUFFER_END

TEXTURECUBE(_Cubemap);
SAMPLER(sampler_Cubemap);


#endif
