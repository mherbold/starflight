
#ifndef UBER_SHADER_FORWARD_BASE
#define UBER_SHADER_FORWARD_BASE

#define IS_FORWARD 1

#include "UberShaderCore.cginc"

UberShaderVertexOutput vertUberForwardBase( UberShaderVertexInput v )
{
	return ComputeVertexShaderOutput( v );
}

float4 fragUberForwardBase( UberShaderVertexOutput i ) : SV_Target
{
	float4 diffuseColor = ComputeDiffuseColor( i );
	float occlusion = ComputeOcclusion( i );
	float4 specular = ComputeSpecular( i );
	float3 normal = ComputeNormal( i );
	float3 emissive = ComputeEmissive( i );

#if OCCLUSION_APPLYTOALBEDO

	diffuseColor *= occlusion;

#endif

	return ComputeLighting( i, diffuseColor, specular, emissive, normal );
}

#endif
