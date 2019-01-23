
#ifndef SF_SHADER_CORE
#define SF_SHADER_CORE

#include "SF - Unity.cginc"
#include "SF - SimplexNoise.cginc"

float4x4 SF_ProjectionMatrix;

sampler2D SF_ScatterMapA;
sampler2D SF_ScatterMapB;
sampler2D SF_DensityMap;

float SF_Density;
float SF_Speed;

sampler2D SF_WaterMaskMap;
float4 SF_WaterMaskMap_ST;

sampler2D _MainTex;
float4 _MainTex_ST;

sampler2D _DetailAlbedoMap;
float4 _DetailAlbedoMap_ST;
float4 SF_AlbedoColor;

sampler2D SF_SpecularMap;
float4 SF_SpecularMap_ST;
float3 SF_SpecularColor;
float SF_Smoothness;

sampler2D SF_NormalMap;
float4 SF_NormalMap_ST;
float SF_NormalMapStrength;

sampler2D SF_DetailNormalMap;
float4 SF_DetailNormalMap_ST;
float SF_DetailNormalMapStrength;

sampler2D SF_EmissiveMap;
float4 SF_EmissiveMap_ST;
float3 SF_EmissiveColor;

sampler2D SF_OcclusionMap;
float4 SF_OcclusionMap_ST;
float SF_OcclusionPower;

float SF_AlphaTestValue;

float4 SF_DepthFadeParams;

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

struct SF_FragmentShaderOutput
{
	half4 color : SV_Target;
	float depth : SV_Depth;
};

SF_VertexShaderOutput ComputeVertexShaderOutput( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	float4 positionWorld = mul( unity_ObjectToWorld, v.position );
	float3 normalWorld = normalize( mul( v.normal, (float3x3) unity_WorldToObject ) );

	o.positionClip = mul( UNITY_MATRIX_VP, positionWorld );
	o.color = v.color;
	o.texCoord0 = float4( v.texCoord0, 0, 1 );
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
	
	#if SF_BEHINDEVERYTHING_ON

		o.texCoord1 = ComputeScreenPos( o.positionClip );

	#endif // SF_BEHINDEVERYTHING_ON

	return o;
}

float4 ComputeDiffuseColor( SF_VertexShaderOutput i )
{
	#if SF_ALBEDOMAP_ON

		float4 albedoMap = tex2D( _MainTex, TRANSFORM_TEX( i.texCoord0, _MainTex ) );

	#else // !SF_ALBEDOMAP_ON

		float4 albedoMap = 1;

	#endif // SF_ALBEDOMAP_ON

	#if SF_DETAILALBEDOMAP_ON

		float4 detailAlbedoMap = tex2D( _DetailAlbedoMap, TRANSFORM_TEX( i.texCoord0, _DetailAlbedoMap ) );

	#else // !SF_DETAILALBEDOMAP_ON

		float4 detailAlbedoMap = 1;

	#endif // SF_DETAILALBEDOMAP_ON

	#if SF_DEPTHFADE_ON

		float d = distance( i.positionWorld, _WorldSpaceCameraPos );

		float nearFade = saturate( ( d - SF_DepthFadeParams.x ) / ( SF_DepthFadeParams.y - SF_DepthFadeParams.x ) );
		float farFade = saturate( ( d - SF_DepthFadeParams.z ) / ( SF_DepthFadeParams.w - SF_DepthFadeParams.z ) );

		float depthFadeAmount = smoothstep( 0, 1, nearFade ) * smoothstep( 1, 0, farFade );

	#else // !SF_DEPTHFADE_ON

		float depthFadeAmount = 1;

	#endif // SF_DEPTHFADE_ON

	return i.color * SF_AlbedoColor * albedoMap * detailAlbedoMap * float4( 1, 1, 1, depthFadeAmount );
}

float ComputeOcclusion( SF_VertexShaderOutput i )
{
	#if SF_OCCLUSIONMAP_ON

		float occlusionMap = tex2D( SF_OcclusionMap, TRANSFORM_TEX( i.texCoord1, SF_OcclusionMap ) );

		occlusionMap = pow( occlusionMap, SF_OcclusionPower );

	#else // !SF_OCCLUSIONMAP_ON

		float occlusionMap = 1;

	#endif // SF_OCCLUSIONMAP_ON

	return occlusionMap;
}

float4 ComputeSpecular( SF_VertexShaderOutput i )
{
	#if SF_SPECULARMAP_ON

		float4 specularMap = tex2D( SF_SpecularMap, TRANSFORM_TEX( i.texCoord0, SF_SpecularMap ) );

	#else // !SF_SPECULARMAP_ON

		float4 specularMap = 1;

	#endif // SF_SPECULARMAP_ON

	return float4( SF_SpecularColor * specularMap.xyz, SF_Smoothness * specularMap.a );
}

float3 ComputeNormal( SF_VertexShaderOutput i )
{
	#if SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON

		#if SF_NORMALMAP_ON

			float4 normalMap = tex2D( SF_NormalMap, TRANSFORM_TEX( i.texCoord0, SF_NormalMap ) );

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

			float4 detailNormalMap = tex2D( SF_DetailNormalMap, TRANSFORM_TEX( i.texCoord0, SF_DetailNormalMap ) );

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

				float2 waterOffset = float2( 0, _Time.x * SF_Speed ) * SF_DetailNormalMap_ST.xy;

				float2 baseTexCoord = TRANSFORM_TEX( i.texCoord0, SF_DetailNormalMap );

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

					float waterMaskMap = tex2D( SF_WaterMaskMap, TRANSFORM_TEX( i.texCoord0, SF_WaterMaskMap ) );

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

			float3 emissiveMap = tex2D( SF_EmissiveMap, TRANSFORM_TEX( i.texCoord0, SF_EmissiveMap ) );

		#endif // SF_EMISSIVEPROJECTION_ON

	#else // !SF_EMISSIVEMAP_ON

		float3 emissiveMap = 0;

	#endif // SF_EMISSIVEMAP_ON

	return SF_EmissiveColor + emissiveMap;
}

#if SF_IS_FORWARD

float4 ComputeLighting( SF_VertexShaderOutput i, float4 diffuseColor, float4 specular, float3 emissive, float3 normal, float fogAmount )
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

		float blinnTerm = pow( saturate( dot( normal, halfAngle ) ), pow( specular.a, 6 ) * 1000 );

		float3 lightSpecular = lightColor * blinnTerm * lerp( 1, 4, saturate( specular.a * 1.5 - 0.5 ) );

		color += lightDiffuse * lightSpecular * specular.rgb;

	#endif // SF_SPECULAR_ON

	color = lerp( color * shadow + emissive, unity_FogColor.rgb, fogAmount );

	#if SF_ALPHA_ON

		return float4( color * diffuseColor.a, diffuseColor.a );

	#else // !SF_ALPHA_ON

		return float4( color, 1 );

	#endif // SF_ALPHA_ON
}

float ComputeFogAmount( SF_VertexShaderOutput i )
{
	if ( unity_FogParams.x == 0 )
	{
		return 0;
	}
	else
	{
		float3 distanceFromEye = distance( i.positionWorld, _WorldSpaceCameraPos.xyz );

		float fogAmount = 1 - saturate( distanceFromEye * unity_FogParams.z + unity_FogParams.w );
	
		// the below is for exp2 mode
		// float fogAmount = unity_FogParams.x * distanceFromEye;
		// fogAmount = 1 - saturate( exp2( -fogAmount * fogAmount ) );

		return fogAmount;
	}
}

#endif // SF_IS_FORWARD

#ifdef SF_FRACTALDETAILS_ON

void DoFractalDetails( SF_VertexShaderOutput i, in out float3 diffuseColor, in out float3 specular, in out float3 normal )
{
	float dc = simplex_turbulence( float4( i.texCoord0.xy * _DetailAlbedoMap_ST.xy, 0, 0 ), 25, 2, 0.95, 6 );

	dc = saturate( dc * 0.3 + 0.7 );

	diffuseColor.rgb *= dc;
	specular.rgb *= dc;

	float dnx = simplex_turbulence( float4( i.texCoord0.xy * _DetailAlbedoMap_ST.xy, 100, 0 ), 25, 2, 0.95, 6 ) * 0.25;
	float dny = simplex_turbulence( float4( i.texCoord0.xy * _DetailAlbedoMap_ST.xy, 200, 0 ), 25, 2, 0.95, 6 ) * 0.25;

	normal.x += dnx;
	normal.y += dny;

	normalize( normal.xyz );
}

#endif

SF_VertexShaderOutput ComputeCloudsVertexShaderOutput( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	o = ComputeVertexShaderOutput( v );

	o.positionClip = UnityApplyLinearShadowBias( UnityClipSpaceShadowCasterPos( v.position, v.normal ) );

	float2 baseTexCoord = o.texCoord0.xy;

	o.texCoord0.xy = baseTexCoord * float2( 10.6, 10 ) + _Time.x * SF_Speed * float2( 0.5, 0.6 ) * _MainTex_ST.xy;
	o.texCoord0.zw = baseTexCoord * float2( 10, 10.5 ) + _Time.x * SF_Speed * float2( 0.75, 0.5 ) * _MainTex_ST.xy;
	o.texCoord1.xy = baseTexCoord * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1.5, 1 ) * _MainTex_ST.xy;
	o.texCoord1.zw = baseTexCoord * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1, 1.2 ) * _MainTex_ST.xy;

	return o;
}

void ComputeCloudsFragmentShaderOutput( SF_VertexShaderOutput i, out float4 diffuseColor, out float4 specular, out float3 normal, out float3 emissive )
{
	float3 h0 = tex2D( _MainTex, i.texCoord0.xy );
	float3 h1 = tex2D( SF_DensityMap, i.texCoord0.zw );
	float3 h2 = tex2D( SF_ScatterMapA, i.texCoord1.xy );
	float3 h3 = tex2D( SF_ScatterMapB, i.texCoord1.zw );

	float3 fbm = saturate( h0 + h1 + h2 + h3 - SF_Density );

	float alpha = saturate( fbm * SF_AlbedoColor.a * 2 );

	diffuseColor = ComputeDiffuseColor( i );
	specular = ComputeSpecular( i );
	normal = ComputeNormal( i );
	emissive = ComputeEmissive( i );

	diffuseColor.a *= alpha;

	#if SF_ALPHATEST_ON

		clip( diffuseColor.a - SF_AlphaTestValue );

	#endif // SF_ALPHATTEST_ON
}

#endif
