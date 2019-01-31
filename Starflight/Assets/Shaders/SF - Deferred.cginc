
#ifndef SF_SHADER_DEFERRED
#define SF_SHADER_DEFERRED

#include "SF - Core.cginc"
#include "SF - Fractals.cginc"

SF_VertexShaderOutput vertDeferred_SF( SF_VertexShaderInput v )
{
	return ComputeVertexShaderOutput( v );
}

void fragDeferred_SF( SF_VertexShaderOutput i, out half4 outGBuffer0 : SV_Target0, out half4 outGBuffer1 : SV_Target1, out half4 outGBuffer2 : SV_Target2, out half4 outGBuffer3 : SV_Target3 )
{
	float4 albedo = ComputeAlbedo( i, SF_AlbedoColor );
	float occlusion = ComputeOcclusion( i );
	float4 specular = ComputeSpecular( i );
	float3 normal = ComputeNormal( i );
	float3 emissive = ComputeEmissive( i );

	#ifdef SF_FRACTALDETAILS_ON

		DoFractalDetails( i, albedo.rgb, specular.rgb, normal );

	#endif // SF_FRACTALDETAILS_ON

	#if SF_EMISSIVEPROJECTION_ON

		emissive *= pow( saturate( dot( normal, float3( 0, 0, -1 ) ) ), 10 ) * 0.25;

	#endif // SF_EMISSIVEPROJECTION_ON

	#if SF_ALPHATEST_ON

		clip( albedo.a - SF_AlphaTestValue );

	#endif // SF_ALPHATTEST_ON

	#if SF_ALBEDOOCCLUSION_ON

		albedo.rgb *= occlusion;

	#endif

	outGBuffer0 = float4( albedo.rgb, occlusion );
	outGBuffer1 = specular;
	outGBuffer2 = float4( normal * 0.5 + 0.5, 1 );

	#if !defined( UNITY_HDR_ON )

		outGBuffer3 = float4( exp2( -emissive ), 1 );

	#else

		outGBuffer3 = float4( emissive, 1 );

	#endif
}

#endif
