
#ifndef SF_SHADER_CLOUDS
#define SF_SHADER_CLOUDS

#include "SF - Core.cginc"

SF_VertexShaderOutput vertClouds_SF( SF_VertexShaderInput v )
{
	return ComputeCloudsVertexShaderOutput( v );
}

#ifdef SF_OVERRIDEDEPTHOUTPUT_ON

SF_FragmentShaderOutput fragClouds_SF( SF_VertexShaderOutput i )
{
	SF_FragmentShaderOutput o;

	float4 albedo;
	float4 specular;
	float3 normal;
	float3 emissive;

	float fogAmount = ComputeFogAmount( i );

	ComputeCloudsFragmentShaderOutput( i, albedo, specular, normal, emissive, SF_AlbedoColor );

	o.color = ComputeLighting( i, albedo, specular, emissive, normal, fogAmount );
	o.depth = 0.0f;

	return o;
}

#else // !SF_OVERRIDEDEPTHOUTPUT_ON

float4 fragClouds_SF( SF_VertexShaderOutput i ) : SV_Target
{
	float4 albedo;
	float4 specular;
	float3 normal;
	float3 emissive;

	float fogAmount = ComputeFogAmount( i );

	ComputeCloudsFragmentShaderOutput( i, albedo, specular, normal, emissive, SF_AlbedoColor );

	return ComputeLighting( i, albedo, specular, emissive, normal, fogAmount );
}

#endif // SF_OVERRIDEDEPTHOUTPUT_ON

#endif
