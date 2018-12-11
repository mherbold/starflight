
#ifndef SF_SHADER_UNLIT
#define SF_SHADER_UNLIT

#include "SF - Core.cginc"

SF_VertexShaderOutput vertUnlit_SF( SF_VertexShaderInput v )
{
	return ComputeVertexShaderOutput( v );
}

float4 fragUnlit_SF( SF_VertexShaderOutput i ) : SV_Target
{
	float4 diffuseColor = ComputeDiffuseColor( i );

#if SF_ALPHATEST_ON

	clip( diffuseColor.a - SF_AlphaTestValue );

#endif // SF_ALPHATTEST_ON

	return float4( diffuseColor.rgb * diffuseColor.a, diffuseColor.a );
}

#endif
