
Shader "Starflight/Z Prepass"
{
	Properties
	{
		SF_AlbedoMap( "Albedo Map", 2D ) = "white" {}
		SF_AlbedoMapScaleOffset( "Albedo Map Scale Offset", Vector ) = ( 1, 1, 0, 0 )
		SF_AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 1 )

		SF_SpecularMap( "Specular Map", 2D ) = "gray" {}
		SF_SpecularColor( "Specular Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
		SF_Smoothness( "Smoothness", Range( 0, 1 ) ) = 0.5

		SF_OcclusionMap( "Occlusion Map", 2D ) = "white" {}
		SF_OcclusionPower( "Occlusion Power", Range( 0, 10 ) ) = 1
		[MaterialToggle] SF_AlbedoOcclusionOn( "Albedo Occlusion", Float ) = 0

		SF_NormalMap( "Normal Map", 2D ) = "bump" {}
		SF_NormalMapScaleOffset( "Normal Map Scale Offset", Vector ) = ( 1, 1, 0, 0 )
		[MaterialToggle] SF_OrthonormalizeOn( "Orthonormalize", Float ) = 0

		SF_EmissiveMap( "Emissive Map", 2D ) = "black" {}
		SF_EmissiveColor( "Emissive Color", Color ) = ( 0, 0, 0, 1 )

		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendDst( "Blend Dst", Float ) = 0
			
		SF_AlphaTestValue( "Alpha Test Value", Range( 0, 1 ) ) = 0
		[MaterialToggle] SF_AlphaTestOn( "Alpha Test", Float ) = 0

		[MaterialToggle] SF_ZWriteOn( "Z Write", Float ) = 1

		[MaterialToggle] SF_ForwardShadowsOn( "Forward Shadows", Float ) = 1

		SF_RenderQueueOffset( "Render Queue Offset", Int ) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		UsePass "Starflight/Standard/SHADOWCASTER"
		UsePass "Starflight/Standard/DEFERRED"

		Pass
		{
			Name "BLENDPREPASS"

			Tags
			{
				"LightMode" = "ForwardBase"
			}

			ZWrite On
			ColorMask 0

			CGPROGRAM

				#pragma target 3.0

				#pragma vertex vertZPrepass_SF
				#pragma fragment fragZPrepass_SF

				#include "SF - ZPrepass.cginc"

			ENDCG
		}

		UsePass "Starflight/Standard/FORWARDBASE"
	}

	CustomEditor "SFShaderGUI"
}
