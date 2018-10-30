
#ifndef UBER_SHADER_CORE
#define UBER_SHADER_CORE

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "UberShaderUnity.cginc"

sampler2D AlbedoMap;
float3 AlbedoColor;

float Alpha;

sampler2D SpecularMap;
float3 SpecularColor;
float Smoothness;

sampler2D OcclusionMap;
float OcclusionPower;

sampler2D NormalMap;
float4 NormalMapScaleOffset;

sampler2D EmissiveMap;
float3 EmissiveColor;

sampler2D WaterMap;
float4 WaterScale;

sampler2D WaterMaskMap;

float3 UILightDirection;
float3 UILightColor;

struct UberShaderVertexInput
{
	float4 position			: POSITION;
	float3 normal			: NORMAL;
	float4 tangent			: TANGENT;
	float2 texCoord0		: TEXCOORD0;
	float2 texCoord1		: TEXCOORD1;
};

struct UberShaderVertexOutput
{
	float4 positionClip		: SV_POSITION;
	float2 texCoord0		: TEXCOORD0;
	float2 texCoord1		: TEXCOORD1;

#if IS_FORWARD

#if SPECULAR_ON

	float3 eyeDir			: TEXCOORD2;

#endif // SPECULAR_ON

#if FORWARDSHADOWS_ON

	float4 shadowCoord		: TEXCOORD3;

#endif // FORWARDSHADOWS_ON

#endif // IS_FORWARD

	float3 normalWorld		: TEXCOORD4;

#if NORMALMAP_ON || WATERMAP_ON

	float3 tangentWorld		: TEXCOORD5;
	float3 binormalWorld	: TEXCOORD6;

#endif // NORMALMAP_ON || WATERMAP_ON
};

UberShaderVertexOutput ComputeVertexShaderOutput( UberShaderVertexInput v )
{
	UberShaderVertexOutput o;

	float4 positionWorld = mul( unity_ObjectToWorld, v.position );
	float3 normalWorld = normalize( mul( v.normal, (float3x3) unity_WorldToObject ) );

	o.positionClip = mul( UNITY_MATRIX_VP, positionWorld );
	o.texCoord0 = v.texCoord0;
	o.texCoord1 = v.texCoord1;

#if IS_FORWARD

#if SPECULAR_ON

	o.eyeDir = normalize( positionWorld.xyz - _WorldSpaceCameraPos );

#endif // SPECULAR_ON
	
#if FORWARDSHADOWS_ON

	o.shadowCoord = mul( unity_WorldToShadow[ 0 ], mul( unity_ObjectToWorld, v.position ) );

#endif // FORWARDSHADOWS_ON

#endif // IS_FORWARD

	o.normalWorld = normalWorld;

#if NORMALMAP_ON || WATERMAP_ON

	float3 tangentWorld = normalize( mul( v.tangent.xyz, (float3x3) unity_WorldToObject ) );
	float3 binormalWorld = cross( normalWorld, tangentWorld ) * v.tangent.w * unity_WorldTransformParams.w;

	o.tangentWorld = tangentWorld;
	o.binormalWorld = binormalWorld;

#endif // NORMALMAP_ON || WATERMAP_ON

	return o;
}

float4 ComputeDiffuseColor( UberShaderVertexOutput i )
{
#if ALBEDOMAP_ON

	float4 albedoMap = tex2D( AlbedoMap, i.texCoord0 );

#else // !ALBEDOMAP_ON

	float4 albedoMap = 1;

#endif // ALBEDOMAP_ON

	return float4( AlbedoColor * albedoMap.rgb, albedoMap.a );
}

float ComputeOcclusion( UberShaderVertexOutput i )
{
#if OCCLUSIONMAP_ON

	float occlusionMap = tex2D( OcclusionMap, i.texCoord1 );

	occlusionMap = pow( occlusionMap, OcclusionPower );

#else // !OCCLUSIONMAP_ON

	float occlusionMap = 1;

#endif // OCCLUSIONMAP_ON

	return occlusionMap;
}

float4 ComputeSpecular( UberShaderVertexOutput i )
{
#if SPECULARMAP_ON

	float3 specularMap = tex2D( SpecularMap, i.texCoord0 );

#else // !SPECULARMAP_ON

	float3 specularMap = 1;

#endif // SPECULARMAP_ON

	return float4( SpecularColor * specularMap.r, Smoothness * specularMap.g );
}

float3 ComputeNormal( UberShaderVertexOutput i )
{
#if NORMALMAP_ON || WATERMAP_ON

#if NORMALMAP_ON

	float4 normalMap = tex2D( NormalMap, i.texCoord0 * NormalMapScaleOffset.xy + NormalMapScaleOffset.zw );

#if NORMALMAP_COMPRESSED

	normalMap.x *= normalMap.w;
	normalMap.xy = ( normalMap.xy * 2 - 1 );
	normalMap.z = sqrt( 1 - saturate( dot( normalMap.xy, normalMap.xy ) ) );

#else // !NORMALMAP_COMPRESSED

	normalMap = normalMap * 2 - 1;

#endif // NORMALMAP_COMPRESSED

	normalMap.xyz = normalize( normalMap.xyz );

#else // !NORMALMAP_ON

	float4 normalMap = 0;

#endif // NORMALMAP_ON

#if WATERMAP_ON

	const float2x2 plus120 = float2x2( -0.5, -0.866, 0.866, -0.5 );
	const float2x2 minus120 = float2x2( -0.5, 0.866, -0.866, -0.5 );

	float2 waterOffset = float2( 0, _Time.x * WaterScale.w );
	float2 scaledTexCoord = i.texCoord0 * WaterScale.xy;

	float2 texCoord = scaledTexCoord + waterOffset;
	float4 waterMapA = tex2D( WaterMap, texCoord );

	texCoord = mul( scaledTexCoord, plus120 ) + waterOffset;
	float4 waterMapB = tex2D( WaterMap, texCoord );

	texCoord = mul( scaledTexCoord, minus120 ) + waterOffset;
	float4 waterMapC = tex2D( WaterMap, texCoord );

#if WATERMAP_COMPRESSED

	waterMapA.x *= waterMapA.w;
	waterMapA.xy = ( waterMapA.xy * 2 - 1 );
	waterMapA.z = sqrt( 1 - saturate( dot( waterMapA.xy, waterMapA.xy ) ) );

	waterMapB.x *= waterMapB.w;
	waterMapB.xy = ( waterMapB.xy * 2 - 1 );
	waterMapB.z = sqrt( 1 - saturate( dot( waterMapB.xy, waterMapB.xy ) ) );

	waterMapC.x *= waterMapC.w;
	waterMapC.xy = ( waterMapC.xy * 2 - 1 );
	waterMapC.z = sqrt( 1 - saturate( dot( waterMapC.xy, waterMapC.xy ) ) );

#else // !WATERMAP_COMPRESSED

	waterMapA = waterMapA * 2 - 1;
	waterMapB = waterMapB * 2 - 1;
	waterMapC = waterMapC * 2 - 1;

#endif // WATERMAP_COMPRESSED

	float3 waterMap = waterMapA.xyz + waterMapB.xyz + waterMapC.xyz;

	waterMap.z *= WaterScale.z;

#if WATERMASKMAP_ON

	float waterMaskMap = tex2D( WaterMaskMap, i.texCoord0 );

	waterMap = lerp( normalMap.xyz, waterMap, waterMaskMap );

#endif // WATERMASKMAP_ON

	normalMap.xyz = normalize( normalMap.xyz + waterMap );

#endif // WATERMAP_ON

	float3 normalWorld = i.normalWorld;
	float3 tangentWorld = i.tangentWorld;
	float3 binormalWorld = i.binormalWorld;

#if TEXTURESPACE_ORTHONORMALIZE

	normalWorld = normalize( i.normalWorld );
	tangentWorld = normalize( tangentWorld - normalWorld * dot( tangentWorld, normalWorld ) );
	float3 normalCrossTangent = cross( normalWorld, tangentWorld );
	binormalWorld = normalCrossTangent * sign( dot( normalCrossTangent, binormalWorld ) );

#endif // TEXTURESPACE_ORTHONORMALIZE

normalWorld = normalize( tangentWorld * normalMap.x + binormalWorld * normalMap.y + normalWorld * normalMap.z );

#else // !NORMALMAP_ON && !WATERMAP_ON

#if TEXTURESPACE_ORTHONORMALIZE

	float3 normalWorld = normalize( i.normalWorld );

#else // !TEXTURESPACE_ORTHONORMALIZE

	float3 normalWorld = i.normalWorld;

#endif // TEXTURESPACE_ORTHONORMALIZE

#endif // NORMALMAP_ON || WATERMAP_ON

	return normalWorld;
}

float3 ComputeEmissive( UberShaderVertexOutput i )
{
#if EMISSIVEMAP_ON

	float3 emissiveMap = tex2D( EmissiveMap, i.texCoord0 );

#else // !EMISSIVEMAP_ON

	float3 emissiveMap = 0;

#endif // EMISSIVEMAP_ON

	return EmissiveColor + emissiveMap;
}

#if IS_FORWARD

float4 ComputeLighting( UberShaderVertexOutput i, float4 diffuseColor, float4 specular, float3 emissive, float3 normal )
{
#if UI_ON

	float3 lightDirectionWorld = UILightDirection;
	float3 lightColor = UILightColor;

#else // !UI_ON

	float3 lightDirectionWorld = _WorldSpaceLightPos0.xyz;
	float3 lightColor = _LightColor0;

#endif // UI_ON

	float3 lightDiffuse = lightColor * saturate( dot( lightDirectionWorld, normal ) );

	float3 color = lightDiffuse * diffuseColor.rgb;

#if FORWARDSHADOWS_ON

	float shadow = UNITY_SAMPLE_SHADOW( _ShadowMapTexture, i.shadowCoord.xyz );

	shadow = _LightShadowData.r + shadow * ( 1 - _LightShadowData.r );

#else // !FORWARDSHADOWS_ON

	float shadow = 1;

#endif // FORWARDSHADOWS_ON

#if SPECULAR_ON

	float3 halfAngle = normalize( lightDirectionWorld - normalize( i.eyeDir ) );

	float blinnTerm = pow( saturate( dot( normal, halfAngle ) ), pow( specular.w, 6 ) * 1000 );

	float3 lightSpecular = lightColor * blinnTerm * lerp( 1, 4, saturate( specular.w * 1.5 - 0.5 ) );

	color += lightDiffuse * lightSpecular * specular.xyz;

#endif // SPECULAR_ON

#if ALPHA_ON

	float alpha = Alpha * diffuseColor.a;

	return float4( ( color * shadow + emissive ) * alpha, alpha );

#else // !ALPHA_ON

	return float4( ( color * shadow + emissive ), 1 );

#endif // ALPHA_ON
}

#endif // IS_FORWARD

#endif
