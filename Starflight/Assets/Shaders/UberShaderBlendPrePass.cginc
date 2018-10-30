
#ifndef UBER_SHADER_BLEND_PRE_PASS
#define UBER_SHADER_BLEND_PRE_PASS

#include "UberShaderCore.cginc"

UberShaderVertexOutput vertUberBlendPrePass( UberShaderVertexInput v )
{
	return ComputeVertexShaderOutput( v );
}

float4 fragUberBlendPrePass( UberShaderVertexOutput i ) : SV_Target
{
	return 1;
}

#endif
