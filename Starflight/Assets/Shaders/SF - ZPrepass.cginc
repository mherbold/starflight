
#ifndef SF_SHADER_Z_PREPASS
#define SF_SHADER_Z_PREPASS

#include "SF - Core.cginc"

SF_VertexShaderOutput vertZPrepass_SF( SF_VertexShaderInput v )
{
	return ComputeVertexShaderOutput( v );
}

float4 fragZPrepass_SF( SF_VertexShaderOutput i ) : SV_Target
{
	return 1;
}

#endif
