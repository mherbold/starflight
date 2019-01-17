
#ifndef SF_SHADER_CORE
#define SF_SHADER_CORE

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "SF - Unity.cginc"
#include "SF - SimplexNoise.cginc"

float4x4 SF_ProjectionMatrix;

sampler2D SF_ScatterMapA;
sampler2D SF_ScatterMapB;
sampler2D SF_DensityMap;

float SF_Density;
float SF_Speed;

sampler2D SF_WaterMaskMap;

sampler2D SF_AlbedoMap;
sampler2D SF_DetailAlbedoMap;
float4 SF_AlbedoColor;

sampler2D SF_SpecularMap;
float3 SF_SpecularColor;
float SF_Smoothness;

sampler2D SF_NormalMap;
float SF_NormalMapStrength;

sampler2D SF_DetailNormalMap;
float SF_DetailNormalMapStrength;

sampler2D SF_EmissiveMap;
float3 SF_EmissiveColor;

float4 SF_BaseScaleOffset;
float4 SF_DetailScaleOffset;

sampler2D SF_OcclusionMap;
float SF_OcclusionPower;

float SF_AlphaTestValue;

struct SF_VertexShaderInput
{
	float4 position			: POSITION;
	float4 color			: COLOR;
	float3 normal			: NORMAL;
	float4 tangent			: TANGENT;
	float2 texCoord0		: TEXCOORD0;
	float2 texCoord1		: TEXCOORD1;
};

struct SF_VertexShaderOutput
{
	float4 positionClip		: SV_POSITION;
	float4 color			: COLOR;
	float4 texCoord0		: TEXCOORD0;
	float4 texCoord1		: TEXCOORD1;
	float4 positionWorld	: TEXCOORD2;
	float3 eyeDir			: TEXCOORD3;

#if SF_IS_FORWARD && SF_FORWARDSHADOWS_ON

	float4 shadowCoord		: TEXCOORD4;

#endif // SF_IS_FORWARD && SF_FORWARDSHADOWS_ON

	float3 normalWorld		: TEXCOORD5;

#if SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON

	float3 tangentWorld		: TEXCOORD6;
	float3 binormalWorld	: TEXCOORD7;

#endif // SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON
};

SF_VertexShaderOutput ComputeVertexShaderOutput( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	float4 positionWorld = mul( unity_ObjectToWorld, v.position );
	float3 normalWorld = normalize( mul( v.normal, (float3x3) unity_WorldToObject ) );

	o.positionClip = mul( UNITY_MATRIX_VP, positionWorld );
	o.color = v.color;
	o.texCoord0 = float4( v.texCoord0 * SF_BaseScaleOffset.xy + SF_BaseScaleOffset.zw, 0, 1 );
	o.texCoord1 = float4( v.texCoord1, 0, 1 );
	o.positionWorld = positionWorld;
	o.eyeDir = normalize( positionWorld.xyz - _WorldSpaceCameraPos );

	#if SF_IS_FORWARD && SF_FORWARDSHADOWS_ON

		o.shadowCoord = mul( unity_WorldToShadow[ 0 ], mul( unity_ObjectToWorld, v.position ) );

	#endif // SF_IS_FORWARD && SF_FORWARDSHADOWS_ON

	o.normalWorld = normalWorld;

	#if SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON

		float3 tangentWorld = normalize( mul( v.tangent.xyz, (float3x3) unity_WorldToObject ) );
		float3 binormalWorld = cross( normalWorld, tangentWorld ) * v.tangent.w * unity_WorldTransformParams.w;

		o.tangentWorld = tangentWorld;
		o.binormalWorld = binormalWorld;

	#endif // SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON

	return o;
}

float4 ComputeDiffuseColor( SF_VertexShaderOutput i )
{
	#if SF_ALBEDOMAP_ON

		float4 albedoMap = tex2D( SF_AlbedoMap, i.texCoord0.xy );

	#else // !SF_ALBEDOMAP_ON

		float4 albedoMap = 1;

	#endif // SF_ALBEDOMAP_ON

	#if SF_DETAILALBEDOMAP_ON

		float4 detailAlbedoMap = tex2D( SF_DetailAlbedoMap, i.texCoord0.xy * SF_DetailScaleOffset.xy + SF_DetailScaleOffset.zw );

	#else // !SF_DETAILALBEDOMAP_ON

		float4 detailAlbedoMap = 1;

	#endif // SF_DETAILALBEDOMAP_ON

	return i.color * SF_AlbedoColor * albedoMap * detailAlbedoMap;
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

		float4 specularMap = tex2D( SF_SpecularMap, i.texCoord0.xy );

	#else // !SF_SPECULARMAP_ON

		float4 specularMap = 1;

	#endif // SF_SPECULARMAP_ON

	return float4( SF_SpecularColor * specularMap.xyz, SF_Smoothness * specularMap.a );
}

float3 ComputeNormal( SF_VertexShaderOutput i )
{
	#if SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON

		#if SF_NORMALMAP_ON

			float4 normalMap = tex2D( SF_NormalMap, i.texCoord0.xy );

			#if SF_NORMALMAP_ISCOMPRESSED

				normalMap.xy = ( normalMap.wy * 2 - 1 );
				normalMap.z = sqrt( 1 - saturate( dot( normalMap.xy, normalMap.xy ) ) );

			#else // !SF_NORMALMAP_ISCOMPRESSED

				normalMap = normalMap * 2 - 1;

			#endif // SF_NORMALMAP_ISCOMPRESSED

			normalMap.xyz = normalize( normalMap.xyz * float3( SF_NormalMapStrength.xx, 1 ) );

		#else // !SF_NORMALMAP_ON

			float4 normalMap = 0;

		#endif // SF_NORMALMAP_ON

		#if SF_DETAILNORMALMAP_ON

			float4 detailNormalMap = tex2D( SF_DetailNormalMap, i.texCoord0.xy * SF_DetailScaleOffset.xy + SF_DetailScaleOffset.zw );

			#if SF_DETAILNORMALMAP_ISCOMPRESSED

				detailNormalMap.xy = ( detailNormalMap.wy * 2 - 1 );
				detailNormalMap.z = sqrt( 1 - saturate( dot( detailNormalMap.xy, detailNormalMap.xy ) ) );

			#else // !SF_DETAILNORMALMAP_ISCOMPRESSED

				detailNormalMap = detailNormalMap * 2 - 1;

			#endif // SF_DETAILNORMALMAP_ISCOMPRESSED

			detailNormalMap.xyz = normalize( detailNormalMap.xyz * float3( SF_DetailNormalMapStrength.xx, 1 ) );

			#if SF_WATER_ON

				const float2x2 plus120 = float2x2( -0.5, -0.866, 0.866, -0.5 );
				const float2x2 minus120 = float2x2( -0.5, 0.866, -0.866, -0.5 );

				float2 waterOffset = float2( 0, _Time.x * SF_Speed ) * SF_DetailScaleOffset.xy;

				float2 baseTexCoord = i.texCoord0.xy * SF_DetailScaleOffset.xy + SF_DetailScaleOffset.zw;

				float2 texCoord = baseTexCoord + waterOffset;
				float4 waterNormalMapA = tex2D( SF_DetailNormalMap, texCoord );

				texCoord = mul( baseTexCoord, plus120 ) + waterOffset;
				float4 waterNormalMapB = tex2D( SF_DetailNormalMap, texCoord );

				texCoord = mul( baseTexCoord, minus120 ) + waterOffset;
				float4 waterNormalMapC = tex2D( SF_DetailNormalMap, texCoord );

				#if SF_DETAILNORMALMAP_ISCOMPRESSED

					waterNormalMapA.xy = ( waterNormalMapA.wy * 2 - 1 );
					waterNormalMapA.z = sqrt( 1 - saturate( dot( waterNormalMapA.xy, waterNormalMapA.xy ) ) );

					waterNormalMapB.xy = ( waterNormalMapB.wy * 2 - 1 );
					waterNormalMapB.z = sqrt( 1 - saturate( dot( waterNormalMapB.xy, waterNormalMapB.xy ) ) );

					waterNormalMapC.xy = ( waterNormalMapC.wy * 2 - 1 );
					waterNormalMapC.z = sqrt( 1 - saturate( dot( waterNormalMapC.xy, waterNormalMapC.xy ) ) );

				#else // !SF_DETAILNORMALMAP_ISCOMPRESSED

					waterNormalMapA = waterMapA * 2 - 1;
					waterNormalMapB = waterMapB * 2 - 1;
					waterNormalMapC = waterMapC * 2 - 1;

				#endif // SF_DETAILNORMALMAP_ISCOMPRESSED

				float4 waterNormalMap = float4( normalize( ( waterNormalMapA.xyz + waterNormalMapB.xyz + waterNormalMapC.xyz ) * float3( SF_DetailNormalMapStrength.xx, 1 ) ), 1 );

				#if SF_WATERMASKMAP_ON

					float waterMaskMap = tex2D( SF_WaterMaskMap, i.texCoord0.xy );

					detailNormalMap.xyz = lerp( detailNormalMap.xyz, waterNormalMap.xyz, waterMaskMap );

				#else // !SF_WATERMASKMAP_ON

					detailNormalMap = waterNormalMap;

				#endif // SF_WATERMASKMAP_ON

			#endif // !SF_WATER_ON

		#else // !SF_DETAILNORMALMAP_ON

			float4 detailNormalMap = 0;

		#endif // SF_DETAILNORMALMAP_ON

		float3 normalWorld = i.normalWorld;
		float3 tangentWorld = i.tangentWorld;
		float3 binormalWorld = i.binormalWorld;

		#if SF_ORTHONORMALIZE_ON

			normalWorld = normalize( i.normalWorld );
			tangentWorld = normalize( tangentWorld - normalWorld * dot( tangentWorld, normalWorld ) );
			float3 normalCrossTangent = cross( normalWorld, tangentWorld );
			binormalWorld = normalCrossTangent * sign( dot( normalCrossTangent, binormalWorld ) );

		#endif // SF_ORTHONORMALIZE_ON

		float3 normalLocal = normalize( normalMap.xyz + detailNormalMap.xyz );

		normalWorld = normalize( tangentWorld * normalLocal.x + binormalWorld * normalLocal.y + normalWorld * normalLocal.z );

	#else // !SF_NORMALMAP_ON && !SF_DETAILNORMAPMAP_ON

		#if SF_ORTHONORMALIZE_ON

			float3 normalWorld = normalize( i.normalWorld );

		#else // !SF_ORTHONORMALIZE_ON

			float3 normalWorld = i.normalWorld;

		#endif // SF_ORTHONORMALIZE_ON

	#endif // SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON

	return normalWorld;
}

float3 ComputeEmissive( SF_VertexShaderOutput i )
{
	#if SF_EMISSIVEMAP_ON

		#if SF_EMISSIVEPROJECTION_ON

			float2 texCoord = mul( i.positionWorld, SF_ProjectionMatrix ).xy;

			float3 emissiveMap = tex2D( SF_EmissiveMap, texCoord );

		#else // !SF_EMISSIVEPROJECTION_ON

			float3 emissiveMap = tex2D( SF_EmissiveMap, i.texCoord0.xy );

		#endif // SF_EMISSIVEPROJECTION_ON

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

#ifdef SF_FRACTALDETAILS_ON

void DoFractalDetails( SF_VertexShaderOutput i, in out float3 diffuseColor, in out float3 specular, in out float3 normal )
{
	float dc = simplex_turbulence( float4( i.texCoord0.xy * SF_DetailScaleOffset.xy, 0, 0 ), 25, 2, 0.95, 6 );

	dc = saturate( dc * 0.3 + 0.7 );

	diffuseColor.rgb *= dc;
	specular.rgb *= dc;

	float dnx = simplex_turbulence( float4( i.texCoord0.xy * SF_DetailScaleOffset.xy, 100, 0 ), 25, 2, 0.95, 6 ) * 0.25;
	float dny = simplex_turbulence( float4( i.texCoord0.xy * SF_DetailScaleOffset.xy, 200, 0 ), 25, 2, 0.95, 6 ) * 0.25;

	normal.x += dnx;
	normal.y += dny;

	normalize( normal.xyz );
}

#endif

#endif
