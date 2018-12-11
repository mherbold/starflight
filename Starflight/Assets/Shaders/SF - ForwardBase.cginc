
#ifndef SF_SHADER_FORWARD_BASE
#define SF_SHADER_FORWARD_BASE

#define SF_IS_FORWARD 1

#include "SF - Core.cginc"

SF_VertexShaderOutput vertForwardBase_SF( SF_VertexShaderInput v )
{
	return ComputeVertexShaderOutput( v );
}

float4 fragForwardBase_SF( SF_VertexShaderOutput i ) : SV_Target
{
	float4 diffuseColor = ComputeDiffuseColor( i );
	float occlusion = ComputeOcclusion( i );
	float4 specular = ComputeSpecular( i );
	float3 normal = ComputeNormal( i );
	float3 emissive = ComputeEmissive( i );

#if SF_ALPHATEST_ON

	clip( diffuseColor.a - SF_AlphaTestValue );

#endif // SF_ALPHATTEST_ON

#if SF_ALBEDOOCCLUSION_ON

	diffuseColor.rgb *= occlusion;

#endif

	return ComputeLighting( i, diffuseColor, specular, emissive, normal );
}

#endif
