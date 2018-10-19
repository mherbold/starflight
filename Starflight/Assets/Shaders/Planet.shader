
Shader "Custom/Planet"
{
	Properties
	{
		_Albedo( "Albedo Map", 2D ) = "black" {}
		_Effects( "Effects Map", 2D ) = "white" {}
		_Normal( "Normal Map", 2D ) = "bump" {}
		_Water( "Water Map", 2D ) = "bump" {}
		_Reflection( "Reflection Map", CUBE ) = "black" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		Pass
		{
			Tags
			{
				"LightMode" = "Deferred"
			}

			CGPROGRAM

				#pragma target 3.0

				#pragma vertex vertPlanetDeferred
				#pragma fragment fragPlanetDeferred

				sampler2D _Albedo;
				sampler2D _Effects;
				sampler2D _Normal;
				sampler2D _Water;
				samplerCUBE _Reflection;

				float4 _WaterScale;
				float4 _WaterOffset;

				struct VertexInputPlanetDeferred
				{
					float3 vertex	: POSITION;
					half3 normal	: NORMAL;
					half4 tangent	: TANGENT;
					float2 uv0		: TEXCOORD0;
				};

				struct VertexOutputPlanetDeferred
				{
					float4 pos				: SV_POSITION;
					float2 tex0				: TEXCOORD0;
					float4 tex1				: TEXCOORD1;
					float3 eyeDir			: TEXCOORD2;
					float3 tangentWorld		: TEXCOORD3;
					float3 binormalWorld	: TEXCOORD4;
					float3 normalWorld		: TEXCOORD5;
				};

				VertexOutputPlanetDeferred vertPlanetDeferred( VertexInputPlanetDeferred v )
				{
					VertexOutputPlanetDeferred o;

					float4 posWorld = mul( unity_ObjectToWorld, float4( v.vertex, 1.0 ) );

					o.pos = mul( UNITY_MATRIX_VP, posWorld );
					o.tex0 = v.uv0.xy;
					o.tex1 = float4( v.uv0.xy, v.uv0.xy ) * _WaterScale + _WaterOffset;
					o.eyeDir = normalize( posWorld.xyz - _WorldSpaceCameraPos );

					float3 normalWorld = normalize( mul( v.normal, (float3x3) unity_WorldToObject ) );
					float3 tangentWorld = normalize( mul( v.tangent.xyz, (float3x3) unity_WorldToObject ) );
					float3 binormalWorld = cross( normalWorld, tangentWorld ) * v.tangent.w * unity_WorldTransformParams.w;

					o.tangentWorld = tangentWorld;
					o.binormalWorld = binormalWorld;
					o.normalWorld = normalWorld;

					return o;
				}

				void fragPlanetDeferred( VertexOutputPlanetDeferred i, out half4 outGBuffer0 : SV_Target0, out half4 outGBuffer1 : SV_Target1, out half4 outGBuffer2 : SV_Target2, out half4 outGBuffer3 : SV_Target3 )
				{
					float3 albedo = tex2D( _Albedo, i.tex0 );
					float3 effects = tex2D( _Effects, i.tex0 );
					float4 normal = tex2D( _Normal, i.tex0 );
					float4 water1 = tex2D( _Water, i.tex1.xy );
					float4 water2 = tex2D( _Water, i.tex1.zw );

					/* this converts from RGB24 to XYZ */
					//normal = normal * 2 - 1;
					/* */

					/* this converts from DXT5NM to XYZ */
					normal.x *= normal.w;
					normal.xy = ( normal.xy * 2 - 1 );
					normal.z = sqrt( 1 - saturate( dot(normal.xy, normal.xy ) ) );
					/* */

					/* this converts from DXT5NM to XYZ */
					water1.x *= water1.w;
					water1.xy = ( water1.xy * 2 - 1 );
					water1.z = sqrt( 1 - saturate( dot( water1.xy, water1.xy ) ) );
					/* */

					/* this converts from DXT5NM to XYZ */
					water2.x *= water2.w;
					water2.xy = ( water2.xy * 2 - 1 );
					water2.z = sqrt( 1 - saturate( dot( water2.xy, water2.xy ) ) );
					/* */

					half3 tangentWorld = i.tangentWorld;
					half3 binormalWorld = i.binormalWorld;
					half3 normalWorld = i.normalWorld;

					/* this part is optional - get rid of it if we run into shader instruction count problems */
					normalWorld = normalize( normalWorld );
					tangentWorld = normalize( tangentWorld - normalWorld * dot( tangentWorld, normalWorld ) );
					half3 normalCrossTangent = cross( normalWorld, tangentWorld );
					binormalWorld = normalCrossTangent * sign( dot(normalCrossTangent, binormalWorld ) );
					/* */

					/* compute world space normal */
					normal = normalize( normal + ( water1 + water2 ) * effects.g );
					
					normalWorld = normalize( tangentWorld * normal.x + binormalWorld * normal.y + normalWorld * normal.z );
					/* */

					/* compute reflection */
					half3 reflectedEyeDir = reflect( i.eyeDir, normalWorld );
					half lod = effects.r * effects.r * effects.r * 6;
					half4 reflection = texCUBElod( _Reflection, float4( reflectedEyeDir, lod ) ) * effects.b;
					/* */

					half3 outDiffuseColor = albedo;
					half outOcclusion = 0;
					half3 outSpecularColor = 0.3;
					half outSmoothness = 1 - effects.r;
					half3 outNormal = normalWorld;
					half3 outEmissive = reflection;

					outGBuffer0 = half4( outDiffuseColor, outOcclusion );
					outGBuffer1 = half4( outSpecularColor, outSmoothness );
					outGBuffer2 = half4( outNormal * 0.5 + 0.5, 1 );

					#if !defined( UNITY_HDR_ON )
						outGBuffer3 = half4( exp2( -outEmissive ), 1 );
					#else
						outGBuffer3 = half4( outEmissive, 1 );
					#endif
				}

			ENDCG
		}

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

				#pragma target 3.0

				#pragma vertex vertPlanetForward
				#pragma fragment fragPlanetForward

				sampler2D _Albedo;
				sampler2D _Effects;
				sampler2D _Normal;
				sampler2D _Water;
				samplerCUBE _Reflection;

				float4 _WaterScale;
				float4 _WaterOffset;

				float3 _SunPosition;

				struct VertexInputPlanetForward
				{
					float3 vertex	: POSITION;
					half3 normal	: NORMAL;
					half4 tangent	: TANGENT;
					float2 uv0		: TEXCOORD0;
				};

				struct VertexOutputPlanetForward
				{
					float4 pos				: SV_POSITION;
					float2 tex0				: TEXCOORD0;
					float4 tex1				: TEXCOORD1;
					float3 eyeDir			: TEXCOORD2;
					float3 tangentWorld		: TEXCOORD3;
					float3 binormalWorld	: TEXCOORD4;
					float3 normalWorld		: TEXCOORD5;
					float3 posWorld			: TEXCOORD6;
				};

				VertexOutputPlanetForward vertPlanetForward( VertexInputPlanetForward v )
				{
					VertexOutputPlanetForward o;

					float4 posWorld = mul( unity_ObjectToWorld, float4( v.vertex, 1.0 ) );

					o.pos = mul( UNITY_MATRIX_VP, posWorld );
					o.tex0 = v.uv0.xy;
					o.tex1 = float4( v.uv0.xy, v.uv0.xy ) * _WaterScale + _WaterOffset;
					o.eyeDir = normalize( posWorld.xyz - _WorldSpaceCameraPos );

					float3 normalWorld = normalize( mul( v.normal, (float3x3) unity_WorldToObject ) );
					float3 tangentWorld = normalize( mul( v.tangent.xyz, (float3x3) unity_WorldToObject ) );
					float3 binormalWorld = cross( normalWorld, tangentWorld ) * v.tangent.w * unity_WorldTransformParams.w;

					o.tangentWorld = tangentWorld;
					o.binormalWorld = binormalWorld;
					o.normalWorld = normalWorld;

					o.posWorld = posWorld;

					return o;
				}

				half4 fragPlanetForward( VertexOutputPlanetForward i ) : SV_Target
				{
					float3 albedo = tex2D( _Albedo, i.tex0 );
					float3 effects = tex2D( _Effects, i.tex0 );
					float4 normal = tex2D( _Normal, i.tex0 );
					float4 water1 = tex2D( _Water, i.tex1.xy );
					float4 water2 = tex2D( _Water, i.tex1.zw );

					/* this converts from RGB24 to XYZ */
					//normal = normal * 2 - 1;
					/* */

					/* this converts from DXT5NM to XYZ */
					normal.x *= normal.w;
					normal.xy = ( normal.xy * 2 - 1 );
					normal.z = sqrt( 1 - saturate( dot(normal.xy, normal.xy ) ) );
					/* */

					/* this converts from DXT5NM to XYZ */
					water1.x *= water1.w;
					water1.xy = ( water1.xy * 2 - 1 );
					water1.z = sqrt( 1 - saturate( dot( water1.xy, water1.xy ) ) );
					/* */

					/* this converts from DXT5NM to XYZ */
					water2.x *= water2.w;
					water2.xy = ( water2.xy * 2 - 1 );
					water2.z = sqrt( 1 - saturate( dot( water2.xy, water2.xy ) ) );
					/* */

					half3 tangentWorld = i.tangentWorld;
					half3 binormalWorld = i.binormalWorld;
					half3 normalWorld = i.normalWorld;

					/* this part is optional - get rid of it if we run into shader instruction count problems */
					normalWorld = normalize( normalWorld );
					tangentWorld = normalize( tangentWorld - normalWorld * dot( tangentWorld, normalWorld ) );
					half3 normalCrossTangent = cross( normalWorld, tangentWorld );
					binormalWorld = normalCrossTangent * sign( dot(normalCrossTangent, binormalWorld ) );
					/* */

					/* compute world space normal */
					normal = normalize( normal + ( water1 + water2 ) * effects.g );
					
					normalWorld = normalize( tangentWorld * normal.x + binormalWorld * normal.y + normalWorld * normal.z );
					/* */

					/* compute reflection */
					half3 reflectedEyeDir = reflect( i.eyeDir, normalWorld );
					half lod = effects.r * effects.r * effects.r * 6;
					half4 reflection = texCUBElod( _Reflection, float4( reflectedEyeDir, lod ) ) * effects.b;
					/* */

					/* light direction (sun is always at 0,0,0) */
					half3 lightDir = normalize( _SunPosition - i.posWorld );
					/* */

					half3 outDiffuseColor = albedo;
					half outOcclusion = 0;
					half3 outSpecularColor = 0.3;
					half outSmoothness = 1 - effects.r;
					half3 outNormal = normalWorld;
					half3 outEmissive = reflection;

					half3 sunlight = 1.5f * dot( lightDir, outNormal );
					half3 specular = pow( saturate( dot( lightDir, reflectedEyeDir ) ), ( 0.5 + outSmoothness ) * 50 );

					half3 finalColor = outDiffuseColor * sunlight + outSpecularColor * specular + outEmissive;

					return half4( finalColor, 1 );
				}

			ENDCG
		}
	}
}
