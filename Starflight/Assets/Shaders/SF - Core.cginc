
#ifndef SF_SHADER_CORE
#define SF_SHADER_CORE

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "SF - Unity.cginc"

sampler2D SF_AlbedoMap;
float4 SF_AlbedoMapScaleOffset;
float4 SF_AlbedoColor;

sampler2D SF_SpecularMap;
float3 SF_SpecularColor;
float SF_Smoothness;

sampler2D SF_OcclusionMap;
float SF_OcclusionPower;

sampler2D SF_NormalMap;
float4 SF_NormalMapScaleOffset;

sampler2D SF_EmissiveMap;
float3 SF_EmissiveColor;

sampler2D SF_WaterMap;
float4 SF_WaterScale;

sampler2D SF_WaterMaskMap;

sampler2D SF_ScatterMapA;
sampler2D SF_ScatterMapB;
sampler2D SF_DensityMap;

float SF_Density;
float SF_Speed;

struct SF_VertexShaderInput
{
	float4 position			: POSITION;
	float4 color			: COLOR;
	float3 normal			: NORMAL;
	float4 tangent			: TANGENT;
	float4 texCoord0		: TEXCOORD0;
	float4 texCoord1		: TEXCOORD1;
};

struct SF_VertexShaderOutput
{
	float4 positionClip		: SV_POSITION;
	float4 color			: COLOR;
	float4 texCoord0		: TEXCOORD0;
	float4 texCoord1		: TEXCOORD1;

#if SF_IS_FORWARD

#if SF_SPECULAR_ON

	float3 eyeDir			: TEXCOORD2;

#endif // SF_SPECULAR_ON

#if SF_FORWARDSHADOWS_ON

	float4 shadowCoord		: TEXCOORD3;

#endif // SF_FORWARDSHADOWS_ON

#endif // SF_IS_FORWARD

	float3 normalWorld		: TEXCOORD4;

#if SF_NORMALMAP_ON || SF_WATERMAP_ON

	float3 tangentWorld		: TEXCOORD5;
	float3 binormalWorld	: TEXCOORD6;

#endif // SF_NORMALMAP_ON || SF_WATERMAP_ON
};

SF_VertexShaderOutput ComputeVertexShaderOutput( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	float4 positionWorld = mul( unity_ObjectToWorld, v.position );
	float3 normalWorld = normalize( mul( v.normal, (float3x3) unity_WorldToObject ) );

	o.positionClip = mul( UNITY_MATRIX_VP, positionWorld );
	o.color = v.color;
	o.texCoord0 = v.texCoord0;
	o.texCoord1 = v.texCoord1;

#if SF_IS_FORWARD

#if SF_SPECULAR_ON

	o.eyeDir = normalize( positionWorld.xyz - _WorldSpaceCameraPos );

#endif // SF_SPECULAR_ON
	
#if SF_FORWARDSHADOWS_ON

	o.shadowCoord = mul( unity_WorldToShadow[ 0 ], mul( unity_ObjectToWorld, v.position ) );

#endif // SF_FORWARDSHADOWS_ON

#endif // SF_IS_FORWARD

	o.normalWorld = normalWorld;

#if SF_NORMALMAP_ON || SF_WATERMAP_ON

	float3 tangentWorld = normalize( mul( v.tangent.xyz, (float3x3) unity_WorldToObject ) );
	float3 binormalWorld = cross( normalWorld, tangentWorld ) * v.tangent.w * unity_WorldTransformParams.w;

	o.tangentWorld = tangentWorld;
	o.binormalWorld = binormalWorld;

#endif // SF_NORMALMAP_ON || SF_WATERMAP_ON

	return o;
}

float4 ComputeDiffuseColor( SF_VertexShaderOutput i )
{
#if SF_ALBEDOMAP_ON

	float4 albedoMap = tex2D( SF_AlbedoMap, i.texCoord0.xy * SF_AlbedoMapScaleOffset.xy + SF_AlbedoMapScaleOffset.zw );

#else // !SF_ALBEDOMAP_ON

	float4 albedoMap = 1;

#endif // SF_ALBEDOMAP_ON

	return i.color * SF_AlbedoColor * albedoMap;
}

float ComputeOcclusion( SF_VertexShaderOutput i )
{
#if SF_OCCLUSIONMAP_ON

	float occlusionMap = tex2D( SF_OcclusionMap, i.texCoord1.xy );

	occlusionMap = pow( occlusionMap, SF_OcclusionPower );

#else // !SF_OCCLUSIONMAP_ON

	float occlusionMap = 1;

#endif // SF_OCCLUSIONMAP_ON

	return occlusionMap;
}

float4 ComputeSpecular( SF_VertexShaderOutput i )
{
#if SF_SPECULARMAP_ON

	float3 specularMap = tex2D( SF_SpecularMap, i.texCoord0.xy );

#else // !SF_SPECULARMAP_ON

	float3 specularMap = 1;

#endif // SF_SPECULARMAP_ON

	return float4( SF_SpecularColor * specularMap.r, SF_Smoothness * specularMap.g );
}

float3 ComputeNormal( SF_VertexShaderOutput i )
{
#if SF_NORMALMAP_ON || SF_WATERMAP_ON

#if SF_NORMALMAP_ON

	float4 normalMap = tex2D( SF_NormalMap, i.texCoord0.xy * SF_NormalMapScaleOffset.xy + SF_NormalMapScaleOffset.zw );

#if SF_NORMALMAP_ISCOMPRESSED

	normalMap.xy = ( normalMap.wy * 2 - 1 );
	normalMap.z = sqrt( 1 - saturate( dot( normalMap.xy, normalMap.xy ) ) );

#else // !SF_NORMALMAP_ISCOMPRESSED

	normalMap = normalMap * 2 - 1;

#endif // SF_NORMALMAP_ISCOMPRESSED

	normalMap.xyz = normalize( normalMap.xyz );

#else // !SF_NORMALMAP_ON

	float4 normalMap = 0;

#endif // SF_NORMALMAP_ON

#if SF_WATERMAP_ON

	const float2x2 plus120 = float2x2( -0.5, -0.866, 0.866, -0.5 );
	const float2x2 minus120 = float2x2( -0.5, 0.866, -0.866, -0.5 );

	float2 waterOffset = float2( 0, _Time.x * SF_WaterScale.w );
	float2 scaledTexCoord = i.texCoord0.xy * SF_WaterScale.xy;

	float2 texCoord = scaledTexCoord + waterOffset;
	float4 waterMapA = tex2D( SF_WaterMap, texCoord );

	texCoord = mul( scaledTexCoord, plus120 ) + waterOffset;
	float4 waterMapB = tex2D( SF_WaterMap, texCoord );

	texCoord = mul( scaledTexCoord, minus120 ) + waterOffset;
	float4 waterMapC = tex2D( SF_WaterMap, texCoord );

#if SF_WATERMAP_ISCOMPRESSED

	waterMapA.xy = ( waterMapA.wy * 2 - 1 );
	waterMapA.z = sqrt( 1 - saturate( dot( waterMapA.xy, waterMapA.xy ) ) );

	waterMapB.xy = ( waterMapB.wy * 2 - 1 );
	waterMapB.z = sqrt( 1 - saturate( dot( waterMapB.xy, waterMapB.xy ) ) );

	waterMapC.xy = ( waterMapC.wy * 2 - 1 );
	waterMapC.z = sqrt( 1 - saturate( dot( waterMapC.xy, waterMapC.xy ) ) );

#else // !SF_WATERMAP_ISCOMPRESSED

	waterMapA = waterMapA * 2 - 1;
	waterMapB = waterMapB * 2 - 1;
	waterMapC = waterMapC * 2 - 1;

#endif // SF_WATERMAP_ISCOMPRESSED

	float3 waterMap = waterMapA.xyz + waterMapB.xyz + waterMapC.xyz;

	waterMap.z *= SF_WaterScale.z;

#if SF_WATERMASKMAP_ON

	float waterMaskMap = tex2D( SF_WaterMaskMap, i.texCoord0.xy );

	waterMap = lerp( normalMap.xyz, waterMap, waterMaskMap );

#endif // SF_WATERMASKMAP_ON

	normalMap.xyz = normalize( normalMap.xyz + waterMap );

#endif // SF_WATERMAP_ON

	float3 normalWorld = i.normalWorld;
	float3 tangentWorld = i.tangentWorld;
	float3 binormalWorld = i.binormalWorld;

#if SF_ORTHONORMALIZE_ON

	normalWorld = normalize( i.normalWorld );
	tangentWorld = normalize( tangentWorld - normalWorld * dot( tangentWorld, normalWorld ) );
	float3 normalCrossTangent = cross( normalWorld, tangentWorld );
	binormalWorld = normalCrossTangent * sign( dot( normalCrossTangent, binormalWorld ) );

#endif // SF_ORTHONORMALIZE_ON

normalWorld = normalize( tangentWorld * normalMap.x + binormalWorld * normalMap.y + normalWorld * normalMap.z );

#else // !SF_NORMALMAP_ON && !SF_WATERMAP_ON

#if SF_ORTHONORMALIZE_ON

	float3 normalWorld = normalize( i.normalWorld );

#else // !SF_ORTHONORMALIZE_ON

	float3 normalWorld = i.normalWorld;

#endif // SF_ORTHONORMALIZE_ON

#endif // SF_NORMALMAP_ON || SF_WATERMAP_ON

	return normalWorld;
}

float3 ComputeEmissive( SF_VertexShaderOutput i )
{
#if SF_EMISSIVEMAP_ON

	float3 emissiveMap = tex2D( SF_EmissiveMap, i.texCoord0.xy );

#else // !SF_EMISSIVEMAP_ON

	float3 emissiveMap = 0;

#endif // SF_EMISSIVEMAP_ON

	return SF_EmissiveColor + emissiveMap;
}

#if SF_IS_FORWARD

float4 ComputeLighting( SF_VertexShaderOutput i, float4 diffuseColor, float4 specular, float3 emissive, float3 normal )
{
	float3 lightDirectionWorld = _WorldSpaceLightPos0.xyz;
	float3 lightColor = _LightColor0;

	float3 lightDiffuse = lightColor * saturate( dot( lightDirectionWorld, normal ) );

	float3 color = lightDiffuse * diffuseColor.rgb;

#if SF_FORWARDSHADOWS_ON

	float shadow = UNITY_SAMPLE_SHADOW( _ShadowMapTexture, i.shadowCoord.xyz );

	shadow = _LightShadowData.r + shadow * ( 1 - _LightShadowData.r );

#else // !SF_FORWARDSHADOWS_ON

	float shadow = 1;

#endif // SF_FORWARDSHADOWS_ON

#if SF_SPECULAR_ON

	float3 halfAngle = normalize( lightDirectionWorld - normalize( i.eyeDir ) );

	float blinnTerm = pow( saturate( dot( normal, halfAngle ) ), pow( specular.w, 6 ) * 1000 );

	float3 lightSpecular = lightColor * blinnTerm * lerp( 1, 4, saturate( specular.w * 1.5 - 0.5 ) );

	color += lightDiffuse * lightSpecular * specular.xyz;

#endif // SF_SPECULAR_ON

#if SF_ALPHA_ON

	return float4( ( color * shadow + emissive ) * diffuseColor.a, diffuseColor.a );

#else // !SF_ALPHA_ON

	return float4( ( color * shadow + emissive ), 1 );

#endif // SF_ALPHA_ON
}

#endif // SF_IS_FORWARD

#endif
