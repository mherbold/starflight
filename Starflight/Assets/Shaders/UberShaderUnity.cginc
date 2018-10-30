
#ifndef UBER_SHADER_UNITY
#define UBER_SHADER_UNITY

#ifndef UNITY_SHADER_VARIABLES_INCLUDED

float4x4	UNITY_MATRIX_VP;
float4x4	unity_ObjectToWorld;
float4x4	unity_WorldToObject;
float4x4	unity_WorldToShadow[ 4 ];
float4		unity_WorldTransformParams;
//float3	_WorldSpaceCameraPos;
float4		_Time;
float4		_LightColor0;
float4		_WorldSpaceLightPos0;

#define ALBEDOMAP_ON					1
#define ALPHA_ON						1
#define SPECULARMAP_ON					1
#define SPECULAR_ON						1
#define OCCLUSIONMAP_ON					1
#define OCCLUSION_APPLYTOALBEDO			1
#define NORMALMAP_ON					1
#define NORMALMAP_COMPRESSED			1
#define TEXTURESPACE_ORTHONORMALIZE		1
#define EMISSIVEMAP_ON					1
#define WATERMAP_ON						1
#define WATERMAP_COMPRESSED				1
#define WATERMASKMAP_ON					1
#define RECEIVESHADOWS_ON				1

#define IS_FORWARD						1
#define FORWARDSHADOW_ON				1

#endif // UNITY_SHADER_VARIABLES_INCLUDED

#if IS_FORWARD && FORWARDSHADOWS_ON

UNITY_DECLARE_SHADOWMAP( _ShadowMapTexture );

#endif // IS_FORWARD && FORWARDSHADOW_ON

sampler3D _DitherMaskLOD;

#endif
