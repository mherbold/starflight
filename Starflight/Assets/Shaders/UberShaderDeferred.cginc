
#ifndef UBER_SHADER_DEFERRED
#define UBER_SHADER_DEFERRED

#include "UberShaderCore.cginc"

UberShaderVertexOutput vertUberDeferred( UberShaderVertexInput v )
{
	return ComputeVertexShaderOutput( v );
}

void fragUberDeferred( UberShaderVertexOutput i, out half4 outGBuffer0 : SV_Target0, out half4 outGBuffer1 : SV_Target1, out half4 outGBuffer2 : SV_Target2, out half4 outGBuffer3 : SV_Target3 )
{
	float3 diffuseColor = ComputeDiffuseColor( i );
	float occlusion = ComputeOcclusion( i );
	float4 specular = ComputeSpecular( i );
	float3 normal = ComputeNormal( i );
	float3 emissive = ComputeEmissive( i );

#if ALBEDOOCCLUSION_ON

	diffuseColor *= occlusion;

#endif

	outGBuffer0 = float4( diffuseColor, occlusion );
	outGBuffer1 = specular;
	outGBuffer2 = float4( normal * 0.5 + 0.5, 1 );

	#if !defined( UNITY_HDR_ON )

		outGBuffer3 = float4( exp2( -emissive ), 1 );

	#else

		outGBuffer3 = float4( emissive, 1 );

	#endif
}

#endif
