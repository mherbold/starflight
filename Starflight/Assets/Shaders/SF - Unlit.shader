
Shader "Starflight/Unlit"
{
	Properties
	{
		/* UV1 Maps */

		SF_AlbedoMap( "Albedo Map", 2D ) = "white" {}
		SF_AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 1 )

		SF_BaseScaleOffset( "Base Scale Offset", Vector ) = ( 1, 1, 0, 0 )

		/* UV2 Maps */

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

		/* Misc Rendering Options */

		[MaterialToggle] SF_ForwardShadowsOn( "Forward Shadows", Float ) = 1

		/* Render Queue Offset */

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

			Cull [SF_CullMode]
			ZWrite On

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
			Name "DEFAULT"

			Lighting Off
			Cull [SF_CullMode]
			Blend [SF_BlendSrc] [SF_BlendDst]
			ZWrite [SF_ZWriteOn]
			ZTest [SF_ZTest]

			CGPROGRAM

				#pragma shader_feature SF_ALBEDOMAP_ON
				#pragma shader_feature SF_ALPHA_ON
				#pragma shader_feature SF_ALPHATEST_ON

				#pragma vertex vertUnlit_SF
				#pragma fragment fragUnlit_SF

				#include "SF - Unlit.cginc"

			ENDCG
		}
	}

	CustomEditor "SFShaderGUI"
}
