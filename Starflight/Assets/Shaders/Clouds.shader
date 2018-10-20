
Shader "Custom/Clouds"
{
	Properties
	{
		_Color( "Color", Color ) = ( 1, 1, 1, 1 )
		_SpecularColor( "Specular Color", Color ) = ( 0.3, 0.3, 0.3, 1 )
		_SpecularPower( "Specular Power", Range( 0.0, 100.0 ) ) = 0.5
		_Density( "Density", Range( 0.0, 2.0 ) ) = 0.2
		_Speed( "Speed", Range( -1.0, 1.0 ) ) = 0.1

		_ScatterMap0( "Scatter Map 1", 2D ) = "white" {}
		_ScatterMap1( "Scatter Map 2", 2D ) = "white" {}
		_DensityMap( "Density Map", 2D ) = "white" {}
		_TextureMap( "Texture Map", 2D ) = "white" {}
		_TextureMap_NRM( "Texture Normal Map", 2D ) = "bump" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}
			
			Blend One OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off

			CGPROGRAM

				#pragma target 3.0

				#pragma vertex vertClouds
				#pragma fragment fragClouds

				float4 _Color;
				float4 _SpecularColor;
				float _SpecularPower;

				sampler2D _ScatterMap0;
				float4 _ScatterMap0_ST;

				sampler2D _ScatterMap1;
				float4 _ScatterMap1_ST;

				sampler2D _DensityMap;
				float4 _DensityMap_ST;

				sampler2D _TextureMap;
				sampler2D _TextureMap_NRM;
				float4 _TextureMap_ST;

				float _Speed;
				float _Density;

				float3 _SunPosition;

				struct VertexInputClouds
				{
					float3 vertex	: POSITION;
					float3 normal	: NORMAL;
					float4 tangent	: TANGENT;
					float2 uv0		: TEXCOORD0;
				};

				struct VertexOutputClouds
				{
					float4 pos				: SV_POSITION;
					float4 tex0				: TEXCOORD0;
					float4 tex1				: TEXCOORD1;
					float3 eyeDir			: TEXCOORD2;
					float3 tangentWorld		: TEXCOORD3;
					float3 binormalWorld	: TEXCOORD4;
					float3 normalWorld		: TEXCOORD5;
					float3 posWorld			: TEXCOORD6;
				};

				VertexOutputClouds vertClouds( VertexInputClouds v )
				{
					VertexOutputClouds o;

					float4 posWorld = mul( unity_ObjectToWorld, float4( v.vertex, 1.0 ) );

					o.pos = mul( UNITY_MATRIX_VP, posWorld );

					#define TRANSFORM_TEX( tex, name ) ( tex.xy * name##_ST.xy + name##_ST.zw )

					o.tex0.xy = TRANSFORM_TEX( v.uv0, _ScatterMap0 ) * float2( 2, 2 ) + _Time.x * _Speed * float2( 1.5, 1.0 );
					o.tex0.zw = TRANSFORM_TEX( v.uv0, _ScatterMap1 ) * float2( 2, 2 ) + _Time.x * _Speed * float2( 1.0, 1.2 );
					o.tex1.xy = TRANSFORM_TEX( v.uv0, _DensityMap ) * float2( 10, 10.5 ) + _Time.x * _Speed * float2( 0.75, 0.5 );
					o.tex1.zw = TRANSFORM_TEX( v.uv0, _TextureMap ) * float2( 10.6, 10 ) + _Time.x * _Speed * float2( 0.5, 0.6 );

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

				float4 fragClouds( VertexOutputClouds i ) : SV_Target
				{
					/* sample height maps */
					float3 h0 = tex2D( _ScatterMap0, i.tex0.xy );
					float3 h1 = tex2D( _ScatterMap1, i.tex0.zw );
					float3 h2 = tex2D( _DensityMap, i.tex1.xy );
					float3 h3 = tex2D( _TextureMap, i.tex1.zw );

					/* sample normal map */
					float4 normal = tex2D( _TextureMap_NRM, i.tex1.zw );

					/* this converts from DXT5NM to XYZ */
					normal.x *= normal.w;
					normal.xy = ( normal.xy * 2 - 1 );
					normal.z = sqrt( 1 - saturate( dot( normal.xy, normal.xy ) ) );

					/* alpha */
					float3 fbm = saturate( h0 + h1 + h2 + h3 - _Density );

					float alpha = saturate( fbm.xyz * _Color.a * 2 );

					/* tangent space */
					float3 tangentWorld = i.tangentWorld;
					float3 binormalWorld = i.binormalWorld;
					float3 normalWorld = i.normalWorld;

					/* this part is optional - get rid of it if we run into shader instruction count problems */
					normalWorld = normalize( normalWorld );
					tangentWorld = normalize( tangentWorld - normalWorld * dot( tangentWorld, normalWorld ) );
					float3 normalCrossTangent = cross( normalWorld, tangentWorld );
					binormalWorld = normalCrossTangent * sign( dot(normalCrossTangent, binormalWorld ) );
					/* */

					/* compute world space normal */
					normalWorld = normalize( tangentWorld * normal.x + binormalWorld * normal.y + normalWorld * normal.z );

					/* diffuse light */
					float3 sunPosition = _SunPosition;

					float3 lightDir = normalize( sunPosition - i.posWorld );

					float3 diffuse = 1.5f * dot( lightDir, normalWorld );

					/* specular light */
					float3 reflectedEyeDir = reflect( i.eyeDir, normalWorld );
					float3 specular = _SpecularColor * pow( saturate( dot( lightDir, reflectedEyeDir ) ), _SpecularPower );

					/* finalize */
					float3 finalColor = _Color * diffuse + specular;

					return float4( finalColor * alpha, alpha );
				}

			ENDCG
		}
	}
}
