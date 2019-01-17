
#ifndef SF_SHADER_UNITY
#define SF_SHADER_UNITY

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
float4		_LightShadowData;

#define UNITY_DECLARE_SHADOWMAP( tex ) Texture2D tex; SamplerComparisonState sampler##tex
#define UNITY_SAMPLE_SHADOW( tex, coord ) tex.SampleCmpLevelZero( sampler##tex, (coord).xy, (coord).z )

float4 UnityClipSpaceShadowCasterPos( float4 vertex, float3 normal ) { return 1; }
float4 UnityApplyLinearShadowBias( float4 clipPos ) { return 1; }

#define SF_ALBEDOMAP_ON					1
#define SF_DETAILALBEDOMAP_ON			1
#define SF_ALPHA_ON						1
#define SF_ALPHATEST_ON					1
#define SF_SPECULARMAP_ON				1
#define SF_SPECULAR_ON					1
#define SF_OCCLUSIONMAP_ON				1
#define SF_ALBEDOOCCLUSION_ON			1
#define SF_NORMALMAP_ON					1
#define SF_NORMALMAP_ISCOMPRESSED		1
#define SF_DETAILNORMALMAP_ON			1
#define SF_DETAILNORMALMAP_ISCOMPRESSED	1
#define SF_ORTHONORMALIZE_ON			1
#define SF_EMISSIVEMAP_ON				1
#define SF_WATERMAP_ON					1
#define SF_WATERMAP_ISCOMPRESSED		1
#define SF_WATERMASKMAP_ON				1

#define SF_IS_FORWARD					1
#define SF_WATER_ON						1
#define SF_EMISSIVEPROJECTION_ON		1
#define SF_FORWARDSHADOWS_ON			1
#define SF_FRACTALDETAILS_ON			1

#endif // UNITY_SHADER_VARIABLES_INCLUDED

#ifndef SHADOWMAPSAMPLER_DEFINED

UNITY_DECLARE_SHADOWMAP( _ShadowMapTexture );

#endif // !SHADOWMAPSAMPLER_DEFINED

sampler3D _DitherMaskLOD;

#endif
