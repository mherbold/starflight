
Shader "Starflight/UberShader-ShadowCasterOff"
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

		UsePass "Starflight/UberShader/DEFERRED"
		UsePass "Starflight/UberShader/FORWARDBASE"
	}

	CustomEditor "UberShaderGUI"
}
