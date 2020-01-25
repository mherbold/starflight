
#ifndef SF_SHADER_FORWARD_BASE
#define SF_SHADER_FORWARD_BASE

#include "SF - Core.cginc"
#include "SF - FractalNoise.cginc"

SF_VertexShaderOutput vertForwardBase_SF( SF_VertexShaderInput v )
{
	return ComputeVertexShaderOutput( v );
}

float4 fragForwardBase_SF( SF_VertexShaderOutput i ) : SV_Target
{
	float4 albedo = ComputeAlbedo( i, SF_AlbedoColor );
	float occlusion = ComputeOcclusion( i );
	float4 specular = ComputeSpecular( i );
	float3 normal = ComputeNormal( i );
	float3 emissive = ComputeEmissive( i );
    float3 reflection = ComputeReflection( i, normal );

	#if SF_FRACTALDETAILS_ON

		float noise = GetFractalNoise( i );

		float albedoModifier = noise * 0.4 + 0.6;
		float specularModifier = saturate( noise * 0.5 + 0.5 );

		#if SF_WATER_ON && SF_WATERMASKMAP_ON

			float waterMaskMap = tex2D( SF_WaterMaskMap, TRANSFORM_TEX(i.texCoord0, SF_WaterMaskMap ) );

			waterMaskMap *= 0.75;

			albedoModifier = lerp( albedoModifier, 1, waterMaskMap );
			specularModifier = lerp( specularModifier, 1, waterMaskMap );

		#endif

		albedo.rgb *= albedoModifier;
		specular.rgb *= specularModifier;

	#endif // SF_FRACTALDETAILS_ON

	#if SF_ALPHATEST_ON

		clip( albedo.a - SF_AlphaTestValue );

	#endif // SF_ALPHATTEST_ON

	#if SF_ALBEDOOCCLUSION_ON

		albedo.rgb *= occlusion;

	#endif // SF_ALBEDOOCCLUSION_ON

	float fogAmount = ComputeFogAmount( i );

	return ComputeLighting( i, albedo, specular, emissive + reflection, normal, fogAmount );
}

#endif
