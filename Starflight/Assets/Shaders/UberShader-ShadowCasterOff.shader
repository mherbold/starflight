
Shader "Starflight/UberShader-ShadowCasterOff"
{
	Properties
	{
		AlbedoMap( "Albedo Map", 2D ) = "white" {}
		AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 1 )

		SpecularMap( "Specular Map", 2D ) = "gray" {}
		SpecularColor( "Specular Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
		Smoothness( "Smoothness", Range( 0, 1 ) ) = 0.5

		OcclusionMap( "Occlusion Map", 2D ) = "white" {}
		OcclusionPower( "Occlusion Power", Range( 0, 10 ) ) = 1
		[MaterialToggle] AlbedoOcclusionOn( "Albedo Occlusion", Float ) = 0

		NormalMap( "Normal Map", 2D ) = "bump" {}
		NormalMapScaleOffset( "Normal Map Scale Offset", Vector ) = ( 1, 1, 0, 0 )
		[MaterialToggle] OrthonormalizeOn( "Orthonormalize", Float ) = 0

		EmissiveMap( "Emissive Map", 2D ) = "black" {}
		EmissiveColor( "Emissive Color", Color ) = ( 0, 0, 0, 1 )

		WaterMap( "Water Map", 2D ) = "bump" {}
		WaterScale( "Water Scale", Vector ) = ( 1, 1, 1, 1 )

		WaterMaskMap( "Water Mask", 2D ) = "white" {}

		[Enum(UnityEngine.Rendering.BlendMode)] BlendSrc( "Blend Src", Float ) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] BlendDst( "Blend Dst", Float ) = 0
		[MaterialToggle] BlendPrePassOn( "Blend Pre-Pass", Float ) = 0

		[MaterialToggle] ShadowCasterOn( "Shadow Caster", Float ) = 1

		[MaterialToggle] LightOverrideOn( "Light Override", Float ) = 0
		LightOverrideDirection( "Light Override Direction", Vector ) = ( 0, 0, 1, 0 )
		[HDR] LightOverrideColor( "Light Override Color", Color ) = ( 1, 1, 1, 1 )
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
