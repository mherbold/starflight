
#ifndef SF_SHADER_UNLIT
#define SF_SHADER_UNLIT

#include "SF - Core.cginc"

SF_VertexShaderOutput vertUnlit_SF( SF_VertexShaderInput v )
{
	return ComputeVertexShaderOutput( v );
}

float4 fragUnlit_SF( SF_VertexShaderOutput i ) : SV_Target
{
	#if SF_BEHINDEVERYTHING_ON

		float sceneZ = LinearEyeDepth( tex2Dproj( _CameraDepthTexture, UNITY_PROJ_COORD( i.texCoord1 ) ).r );

		clip( sceneZ - 2047 );

	#endif // SF_BEHINDEVERYTHING_ON

	float4 albedo = ComputeAlbedo( i, SF_AlbedoColor );

	#if SF_ALPHATEST_ON

		clip( albedo.a - SF_AlphaTestValue );

	#endif // SF_ALPHATTEST_ON

	return float4( albedo.rgb * albedo.a, albedo.a );
}

#endif
