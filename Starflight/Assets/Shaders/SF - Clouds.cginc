
#ifndef SF_SHADER_CLOUDS
#define SF_SHADER_CLOUDS

#define SF_IS_FORWARD 1

#include "SF - Core.cginc"

SF_VertexShaderOutput vertClouds_SF( SF_VertexShaderInput v )
{
	return ComputeCloudsVertexShaderOutput( v );
}

#ifdef SF_OVERRIDEDEPTHOUTPUT_ON

SF_FragmentShaderOutput fragClouds_SF( SF_VertexShaderOutput i )
{
	SF_FragmentShaderOutput o;

	float4 diffuseColor;
	float4 specular;
	float3 normal;
	float3 emissive;

	ComputeCloudsFragmentShaderOutput( i, diffuseColor, specular, normal, emissive );

	o.color = ApplyFog( i, ComputeLighting( i, diffuseColor, specular, emissive, normal ) );
	o.depth = 0.0f;

	return o;
}

#else // !SF_OVERRIDEDEPTHOUTPUT_ON

float4 fragClouds_SF( SF_VertexShaderOutput i ) : SV_Target
{
	float4 diffuseColor;
	float4 specular;
	float3 normal;
	float3 emissive;

	ComputeCloudsFragmentShaderOutput( i, diffuseColor, specular, normal, emissive );

	return ApplyFog( i, ComputeLighting( i, diffuseColor, specular, emissive, normal ) );

	return diffuseColor;
}

#endif // SF_OVERRIDEDEPTHOUTPUT_ON

#endif
