
#ifndef SF_SHADER_CLOUDS_SHADOW_CASTER
#define SF_SHADER_CLOUDS_SHADOW_CASTER

#include "SF - Core.cginc"

SF_VertexShaderOutput vertCloudsShadowCaster_SF( SF_VertexShaderInput v )
{
	return ComputeCloudsVertexShaderOutput( v );
}

float4 fragCloudsShadowCaster_SF( float4 svPosition : SV_POSITION, SF_VertexShaderOutput i ) : SV_Target
{
	#if SF_ALPHA_ON

		float4 diffuseColor;
		float4 specular;
		float3 normal;
		float3 emissive;

		ComputeCloudsFragmentShaderOutput( i, diffuseColor, specular, normal, emissive );

		float alphaRef = tex3D( _DitherMaskLOD, float3( svPosition.xy * 0.25, diffuseColor.a * 0.9375 ) ).a;

		clip( alphaRef - 0.01 );

	#endif // SF_ALPHA_ON

	return 1;
}

#endif
