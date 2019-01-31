
#ifndef SF_SHADER_UNITY
#define SF_SHADER_UNITY

#include "Unity\UnityCG.cginc"
#include "Unity\Lighting.cginc"
#include "Unity\AutoLight.cginc"

UNITY_DECLARE_SHADOWMAP( _ShadowMapTexture );
UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );

sampler3D _DitherMaskLOD;
float _ShadowIntensity;

#endif
