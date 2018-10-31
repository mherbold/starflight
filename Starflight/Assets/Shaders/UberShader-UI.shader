
Shader "Starflight/UberShader-UI"
{
	Properties
	{
		AlbedoMap( "Albedo Map", 2D ) = "white" {}
		AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 1 )

		[Enum(UnityEngine.Rendering.BlendMode)] BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] BlendDst( "Blend Dst", Float ) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Pass
		{
			Name "DEFAULT"

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest [unity_GUIZTestMode]
			Blend [BlendSrc] [BlendDst]

			CGPROGRAM

				#pragma shader_feature ALBEDOMAP_ON
				#pragma shader_feature ALPHA_ON

				#pragma vertex vertUberUI
				#pragma fragment fragUberUI

				#include "UberShaderCore.cginc"

				UberShaderVertexOutput vertUberUI( UberShaderVertexInput v )
				{
					return ComputeVertexShaderOutput( v );
				}

				float4 fragUberUI( UberShaderVertexOutput i ) : SV_Target
				{
					float4 diffuseColor = ComputeDiffuseColor( i );
					
#if ALPHA_ON

					float alpha = AlbedoColor.a * diffuseColor.a;

					return float4( diffuseColor.rgb * alpha, alpha );

#else // !ALPHA_ON

					return float4( diffuseColor.rgb, 1 );

#endif // ALPHA_ON
				}

			ENDCG
		}
	}

	CustomEditor "UberShaderGUI"
}
