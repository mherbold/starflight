
Shader "Starflight/Unlit"
{
	Properties
	{
		SF_AlbedoMap( "Albedo Map", 2D ) = "white" {}
		SF_AlbedoMapScaleOffset( "Albedo Map Scale Offset", Vector ) = ( 1, 1, 0, 0 )
		SF_AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 1 )

		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] SF_BlendDst( "Blend Dst", Float ) = 0

		[MaterialToggle] SF_ZWriteOn( "Z Write", Float ) = 1

		SF_RenderQueueOffset( "Render Queue Offset", Int ) = 0
	}

	SubShader
	{
		Tags
		{
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
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

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON

				#pragma vertex vertShadowCaster_SF
				#pragma fragment fragShadowCaster_SF

				#include "SF - ShadowCaster.cginc"

			ENDCG
		}

		Pass
		{
			Name "DEFAULT"

			Lighting Off
			ZWrite [SF_ZWriteOn]
			ZTest [unity_GUIZTestMode]
			Blend [SF_BlendSrc] [SF_BlendDst]

			CGPROGRAM

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON

				#pragma vertex vertUnlit_SF
				#pragma fragment fragUnlit_SF

				#include "SF - Unlit.cginc"

			ENDCG
		}
	}

	CustomEditor "SFShaderGUI"
}
