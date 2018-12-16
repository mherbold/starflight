
Shader "Starflight/Standard"
{
	Properties
	{
		/* UV1 Maps */

		SF_AlbedoMap( "Albedo Map", 2D ) = "white" {}
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

		SF_BaseScaleOffset( "Base Scale Offset", Vector ) = ( 1, 1, 0, 0 )
		SF_DetailScaleOffset( "Detail Scale Offset", Vector ) = ( 1, 1, 0, 0 )

		/* UV2 Maps */

		SF_OcclusionMap( "Occlusion Map", 2D ) = "white" {}
		SF_OcclusionPower( "Occlusion Power", Range( 0, 10 ) ) = 1
		[MaterialToggle] SF_AlbedoOcclusionOn( "Albedo Occlusion", Float ) = 0

		/* Blending Options */

		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendDst( "Blend Dst", Float ) = 0

		/* Alpha Testing Options */

		SF_AlphaTestValue( "Alpha Test Value", Range( 0, 1 ) ) = 0

		/* Depth Buffer Options */

		[MaterialToggle] SF_ZWriteOn( "Z Write", Float ) = 1

		/* Misc Rendering Options */

		[MaterialToggle] SF_OrthonormalizeOn( "Orthonormalize", Float ) = 0
		[MaterialToggle] SF_EmissiveProjectionOn( "Emissive Projection", Float ) = 0
		[MaterialToggle] SF_ForwardShadowsOn( "Forward Shadows", Float ) = 1
		SF_RenderQueueOffset( "Render Queue Offset", Int ) = 0
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

			ZWrite On
			ZTest LEqual

			CGPROGRAM

				#pragma target 3.0

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON
				#pragma shader_feature SF_ALPHATEST_ON

				#pragma vertex vertShadowCaster_SF
				#pragma fragment fragShadowCaster_SF

				#include "SF - ShadowCaster.cginc"

			ENDCG
		}

		Pass
		{
			Name "DEFERRED"

			Tags
			{
				"LightMode" = "Deferred"
			}

			ZWrite [SF_ZWriteOn]

			CGPROGRAM

				#pragma target 3.0

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON
				#pragma shader_feature SF_ALPHATEST_ON
				#pragma shader_feature SF_SPECULARMAP_ON
				#pragma shader_feature SF_OCCLUSIONMAP_ON
				#pragma shader_feature SF_ALBEDOOCCLUSION_ON
				#pragma shader_feature SF_NORMALMAP_ON
				#pragma shader_feature SF_NORMALMAP_ISCOMPRESSED
				#pragma shader_feature SF_DETAILNORMALMAP_ON
				#pragma shader_feature SF_DETAILNORMALMAP_ISCOMPRESSED
				#pragma shader_feature SF_ORTHONORMALIZE_ON
				#pragma shader_feature SF_EMISSIVEMAP_ON
				#pragma shader_feature SF_EMISSIVEPROJECTION_ON

				#pragma vertex vertDeferred_SF
				#pragma fragment fragDeferred_SF

				#include "SF - Deferred.cginc"

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

			Blend [SF_BlendSrc] [SF_BlendDst]
			ZWrite [SF_ZWriteOn]

			CGPROGRAM

				#pragma target 3.0
				#pragma multi_compile_fwdbase

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON
				#pragma shader_feature SF_ALPHATEST_ON
				#pragma shader_feature SF_SPECULARMAP_ON
				#pragma shader_feature SF_SPECULAR_ON
				#pragma shader_feature SF_OCCLUSIONMAP_ON
				#pragma shader_feature SF_ALBEDOOCCLUSION_ON
				#pragma shader_feature SF_NORMALMAP_ON
				#pragma shader_feature SF_NORMALMAP_ISCOMPRESSED
				#pragma shader_feature SF_DETAILNORMALMAP_ON
				#pragma shader_feature SF_DETAILNORMALMAP_ISCOMPRESSED
				#pragma shader_feature SF_ORTHONORMALIZE_ON
				#pragma shader_feature SF_EMISSIVEMAP_ON
				#pragma shader_feature SF_EMISSIVEPROJECTION_ON
				#pragma shader_feature SF_FORWARDSHADOWS_ON

				#pragma vertex vertForwardBase_SF
				#pragma fragment fragForwardBase_SF

				#include "SF - ForwardBase.cginc"

			ENDCG
		}
	}

	CustomEditor "SFShaderGUI"
}
