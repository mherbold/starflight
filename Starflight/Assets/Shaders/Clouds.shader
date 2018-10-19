
Shader "Custom/Clouds"
{
	Properties
	{
		_Color( "Color", Color ) = ( 1, 1, 1, 1 )
		_Density( "Density", Range( 0.0, 2.0 ) ) = 0.2
		_Speed( "Speed", Range( -1.0, 1.0 ) ) = 0.1

		_ScatterMap0( "Scatter Map 1", 2D ) = "white" {}
		_ScatterMap1( "Scatter Map 2", 2D ) = "white" {}

		_DensityMap( "Density Map", 2D ) = "white" {}
		_DensityMap_NRM( "Density Normal Map", 2D ) = "bump" {}

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

				fixed4 _Color;

				sampler2D _ScatterMap0;
				float4 _ScatterMap0_ST;

				sampler2D _ScatterMap1;
				float4 _ScatterMap1_ST;

				sampler2D _DensityMap;
				float4 _DensityMap_ST;

				sampler2D _TextureMap;
				float4 _TextureMap_ST;

				sampler2D _DensityMap_NRM;
				sampler2D _TextureMap_NRM;

				fixed _Speed;
				fixed _Density;

				float3 _SunPosition;

				struct VertexInputClouds
				{
					float3 vertex	: POSITION;
					half3 normal	: NORMAL;
					half4 tangent	: TANGENT;
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

					o.tex0.xy = TRANSFORM_TEX( v.uv0, _ScatterMap0 ) * fixed2( 2, 2 ) + _Time.x * _Speed * fixed2( 1.5, 1.0 );
					o.tex0.zw = TRANSFORM_TEX( v.uv0, _ScatterMap1 ) * fixed2( 2, 2 ) + _Time.x * _Speed * fixed2( 1.0, 1.2 );
					o.tex1.xy = TRANSFORM_TEX( v.uv0, _DensityMap ) * fixed2( 10, 10.5 ) + _Time.x * _Speed * fixed2( 0.75, 0.5 );
					o.tex1.zw = TRANSFORM_TEX( v.uv0, _TextureMap ) * fixed2( 10.6, 10 ) + _Time.x * _Speed * fixed2( 0.5, 0.6 );

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

				half4 fragClouds( VertexOutputClouds i ) : SV_Target
				{
					/* sample height maps */
					fixed3 h0 = tex2D( _ScatterMap0, i.tex0.xy );
					fixed3 h1 = tex2D( _ScatterMap1, i.tex0.zw );
					fixed3 h2 = tex2D( _DensityMap, i.tex1.xy );
					fixed3 h3 = tex2D( _TextureMap, i.tex1.zw );

					/* sample normal maps */
					fixed4 n0 = tex2D( _DensityMap_NRM, i.tex1.xy );
					fixed4 n1 = tex2D( _TextureMap_NRM, i.tex1.zw );

					/* this converts from DXT5NM to XYZ */
					n0.x *= n0.w;
					n0.xy = ( n0.xy * 2 - 1 );
					n0.z = sqrt( 1 - saturate( dot( n0.xy, n0.xy ) ) );

					/* this converts from DXT5NM to XYZ */
					n1.x *= n1.w;
					n1.xy = ( n1.xy * 2 - 1 );
					n1.z = sqrt( 1 - saturate( dot( n1.xy, n1.xy ) ) );

					/* alpha */
					fixed3 fbm = saturate( h0 + h1 + h2 + h3 - _Density );

					fixed alpha = saturate( fbm.xyz * _Color.a * 2 );

					/* normal */
					fixed3 normal = normalize( n0 + n1 );

					/* tangent space */
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
					normalWorld = normalize( tangentWorld * normal.x + binormalWorld * normal.y + normalWorld * normal.z );

					/* diffuse light */
					float3 sunPosition = _SunPosition;

					half3 lightDir = normalize( sunPosition - i.posWorld );

					half3 diffuse = 1.5f * dot( lightDir, normalWorld );

					/* specular light */
					half3 reflectedEyeDir = reflect( i.eyeDir, normalWorld );
					half3 specular = pow( saturate( dot( lightDir, reflectedEyeDir ) ), 10 );

					/* finalize */
					half3 finalColor = _Color * diffuse + specular;

					return half4( finalColor * alpha, alpha );
				}

			ENDCG
		}
	}
}
