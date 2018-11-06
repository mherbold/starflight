
Shader "Starflight/Clouds"
{
	Properties
	{
		SF_AlbedoMap( "Albedo Map", 2D ) = "white" {}
		SF_AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 1 )

		SF_SpecularMap("Specular Map", 2D) = "gray" {}
		SF_SpecularColor( "Specular Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
		SF_Smoothness( "Smoothness", Range( 0, 1 ) ) = 0.5

		SF_NormalMap( "Normal Map", 2D ) = "bump" {}
		SF_NormalMapScaleOffset( "Normal Map Scale Offset", Vector ) = ( 1, 1, 0, 0 )
		[MaterialToggle] SF_OrthonormalizeOn( "Orthonormalize", Float ) = 0

		SF_ScatterMapA( "Scatter Map A", 2D ) = "black" {}
		SF_ScatterMapB( "Scatter Map B", 2D ) = "black" {}

		SF_DensityMap( "Density Map", 2D ) = "black" {}

		SF_Density( "Density", Range( 0, 2 ) ) = 1
		SF_Speed( "Speed", Range( -1, 1 ) ) = 0.1

		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendDst( "Blend Dst", Float ) = 0

		[MaterialToggle] SF_ZWriteOn( "Z Write", Float ) = 1

		[MaterialToggle] SF_ForwardShadowsOn( "Forward Shadows", Float ) = 1

		SF_RenderQueueOffset( "Render Queue Offset", Int ) = 0
	}

	SubShader
	{
		Pass
		{
			Name "FORWARDBASE"

			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			Cull Off
			ZWrite [SF_ZWriteOn]
			Blend [SF_BlendSrc] [SF_BlendDst]

			CGPROGRAM

				#pragma target 3.0

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON
				#pragma shader_feature SF_SPECULAR_ON
				#pragma shader_feature SF_NORMALMAP_ON
				#pragma shader_feature SF_NORMALMAP_ISCOMPRESSED
				#pragma shader_feature SF_ORTHONORMALIZE_ON
				#pragma shader_feature SF_FORWARDSHADOWS_ON

				#pragma vertex vertClouds_SF
				#pragma fragment fragClouds_SF

				#include "SF - Clouds.cginc"

			ENDCG
		}
	}

	CustomEditor "SFShaderGUI"
}
