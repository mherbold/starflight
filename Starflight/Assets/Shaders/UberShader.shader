
Shader "Starflight/UberShader"
{
	Properties
	{
		AlbedoMap( "Albedo Map", 2D ) = "white" {}
		AlbedoColor( "AlbedoColor", Color ) = ( 1, 1, 1, 1 )

		Alpha( "Alpha", Range( 0, 1 ) ) = 1

		SpecularMap( "Specular Map", 2D ) = "gray" {}
		SpecularColor( "Specular Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
		Smoothness( "Smoothness", Range( 0, 1 ) ) = 0.5

		OcclusionMap( "Occlusion Map", 2D ) = "white" {}
		OcclusionPower( "Occlusion Power", Range( 0, 10 ) ) = 1
		[MaterialToggle] ApplyOcclusionToAlbedo( "Apply Occlusion to Albedo", Float ) = 0

		NormalMap( "Normal Map", 2D ) = "bump" {}
		NormalMapScaleOffset( "Normal Map Scale Offset", Vector ) = ( 1, 1, 0, 0 )
		[MaterialToggle] OrthonormalizeTextureSpace( "Orthonormalize Texture Space", Float ) = 0

		EmissiveMap( "Emissive Map", 2D ) = "black" {}
		EmissiveColor( "Emissive Color", Color ) = ( 0, 0, 0, 1 )

		WaterMap( "Water Map", 2D ) = "bump" {}
		WaterScale( "Water Scale", Vector ) = ( 1, 1, 1, 1 )

		WaterMaskMap( "Water Mask", 2D ) = "white" {}

		[Enum(UnityEngine.Rendering.BlendMode)] BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] BlendDst( "Blend Dst", Float ) = 0
		[MaterialToggle] BlendPrePass( "BlendPrePass", Float ) = 0

		[MaterialToggle] CastShadows( "Cast Shadows", Float ) = 1

		[MaterialToggle] UIToggle( "UI Toggle", Float ) = 0
		UILightDirection( "UI Light Direction", Vector ) = ( 0, 0, 1, 0 )
		[HDR] UILightColor( "UI Light Color", Color ) = ( 1, 1, 1, 1 )
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

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

				#pragma shader_feature ALBEDOMAP_ON
				#pragma shader_feature ALPHA_ON

				#pragma vertex vertUberShadowCaster
				#pragma fragment fragUberShadowCaster

				#include "UberShaderShadowCaster.cginc"

			ENDCG
		}

		Pass
		{
			Name "DEFERRED"

			Tags
			{
				"LightMode" = "Deferred"
			}

			CGPROGRAM

				#pragma target 3.0

				#pragma shader_feature ALBEDOMAP_ON
				#pragma shader_feature SPECULARMAP_ON
				#pragma shader_feature OCCLUSIONMAP_ON
				#pragma shader_feature OCCLUSION_APPLYTOALBEDO
				#pragma shader_feature NORMALMAP_ON
				#pragma shader_feature NORMALMAP_COMPRESSED
				#pragma shader_feature TEXTURESPACE_ORTHONORMALIZE
				#pragma shader_feature EMISSIVEMAP_ON
				#pragma shader_feature WATERMAP_ON
				#pragma shader_feature WATERMAP_COMPRESSED
				#pragma shader_feature WATERMASKMAP_ON

				#pragma vertex vertUberDeferred
				#pragma fragment fragUberDeferred

				#include "UberShaderDeferred.cginc"

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

			Blend [BlendSrc] [BlendDst]

			CGPROGRAM

				#pragma target 3.0

				#pragma shader_feature ALBEDOMAP_ON
				#pragma shader_feature ALPHA_ON
				#pragma shader_feature SPECULARMAP_ON
				#pragma shader_feature SPECULAR_ON
				#pragma shader_feature OCCLUSIONMAP_ON
				#pragma shader_feature OCCLUSION_APPLYTOALBEDO
				#pragma shader_feature NORMALMAP_ON
				#pragma shader_feature NORMALMAP_COMPRESSED
				#pragma shader_feature TEXTURESPACE_ORTHONORMALIZE
				#pragma shader_feature EMISSIVEMAP_ON
				#pragma shader_feature WATERMAP_ON
				#pragma shader_feature WATERMAP_COMPRESSED
				#pragma shader_feature WATERMASKMAP_ON
				#pragma shader_feature UI_ON
				#pragma shader_feature FORWARDSHADOWS_ON

				#pragma vertex vertUberForwardBase
				#pragma fragment fragUberForwardBase

				#include "UberShaderForwardBase.cginc"

			ENDCG
		}
	}

	CustomEditor "UberShaderGUI"
}
