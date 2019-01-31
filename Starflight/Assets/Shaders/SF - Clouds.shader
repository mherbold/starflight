
Shader "Starflight/Clouds"
{
	Properties
	{
		/* Custom Stuff */

		[NoScaleOffset] SF_ScatterMapA( "Scatter Map A", 2D ) = "black" {}
		[NoScaleOffset] SF_ScatterMapB( "Scatter Map B", 2D ) = "black" {}
		[NoScaleOffset] SF_DensityMap( "Density Map", 2D ) = "black" {}
		SF_Density( "Density", Range( 0, 2 ) ) = 1
		SF_Speed( "Speed", Range( -1, 1 ) ) = 0.1

		/* UV1 Maps */

		_MainTex( "Albedo Map", 2D ) = "white" {}
		_DetailAlbedoMap( "Detail Albedo Map", 2D ) = "white" {}
		SF_AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 1 )

		SF_SpecularMap( "Specular Map", 2D ) = "gray" {}
		SF_SpecularColor( "Specular Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
		SF_Smoothness( "Smoothness", Range( 0, 1 ) ) = 0.5

		SF_NormalMap( "Normal Map", 2D ) = "bump" {}
		SF_NormalMapStrength( "Normal Map Strength", Range( 0, 10 ) ) = 1

		SF_DetailNormalMap( "Detail Normal Map", 2D ) = "bump" {}
		SF_DetailNormalMapStrength( "Detail Normal Map Strength", Range( 0, 10 ) ) = 1

		SF_EmissiveMap( "Emissive Map", 2D ) = "black" {}
		SF_EmissiveColor( "Emissive Color", Color ) = ( 0, 0, 0, 1 )

		/* UV2 Maps */

		SF_OcclusionMap( "Occlusion Map", 2D ) = "white" {}
		SF_OcclusionPower( "Occlusion Power", Range( 0, 10 ) ) = 1
		[MaterialToggle] SF_AlbedoOcclusionOn( "Albedo Occlusion", Float ) = 0

		/* Culling Options */

		[Enum(UnityEngine.Rendering.CullMode)] SF_CullMode( "Cull Mode", Float ) = 2

		/* Blending Options */

		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendDst( "Blend Dst", Float ) = 0

		/* Alpha Testing Options */

		SF_AlphaTestValue( "Alpha Test Value", Range( 0, 1 ) ) = 0

		/* Depth Buffer Options */

		[MaterialToggle] SF_ZWriteOn( "Z Write", Float ) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] SF_ZTest( "Z Test", Float ) = 4
		[MaterialToggle] SF_OverrideDepthOutput( "Override Depth Output", Float ) = 0

		/* Misc Rendering Options */

		[MaterialToggle] SF_OrthonormalizeOn( "Orthonormalize", Float ) = 0
	}

	SubShader
	{
		Pass
		{
			Name "SHADOWCASTER"

			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			Cull [SF_CullMode]
			ZWrite On
			ZTest LEqual

			CGPROGRAM

				#pragma target 3.0

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_DETAILALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON
				#pragma shader_feature SF_ALPHATEST_ON

				#pragma vertex vertCloudsShadowCaster_SF
				#pragma fragment fragCloudsShadowCaster_SF

				#include "SF - CloudsShadowCaster.cginc"

			ENDCG
		}

		Pass
		{
			Name "FORWARDBASE"

			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			Cull [SF_CullMode]
			Blend [SF_BlendSrc] [SF_BlendDst]
			ZWrite [SF_ZWriteOn]
			ZTest [SF_ZTest]

			CGPROGRAM

				#pragma target 3.0

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_DETAILALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON
				#pragma shader_feature SF_ALPHATEST_ON
				#pragma shader_feature SF_SPECULAR_ON
				#pragma shader_feature SF_NORMALMAP_ON
				#pragma shader_feature SF_NORMALMAP_ISCOMPRESSED
				#pragma shader_feature SF_OVERRIDEDEPTHOUTPUT_ON
				#pragma shader_feature SF_ORTHONORMALIZE_ON

				#pragma vertex vertClouds_SF
				#pragma fragment fragClouds_SF

				#include "SF - Clouds.cginc"

			ENDCG
		}
	}

	CustomEditor "SFShaderGUI"
}
