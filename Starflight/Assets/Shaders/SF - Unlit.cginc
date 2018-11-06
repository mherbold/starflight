
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
					
#if SF_ALPHA_ON

	float alpha = SF_AlbedoColor.a * diffuseColor.a;

	return float4( diffuseColor.rgb * alpha, alpha );

#else // !SF_ALPHA_ON

	return float4( diffuseColor.rgb, 1 );

#endif // SF_ALPHA_ON
}

#endif
