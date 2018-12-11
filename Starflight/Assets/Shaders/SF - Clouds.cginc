
#ifndef SF_SHADER_CLOUDS
#define SF_SHADER_CLOUDS

#define SF_IS_FORWARD 1

#include "SF - Core.cginc"

SF_VertexShaderOutput vertClouds_SF( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o = ComputeVertexShaderOutput( v );

	o.texCoord0.xy = v.texCoord0.xy * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1.5, 1 );
	o.texCoord0.zw = v.texCoord0.xy * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1, 1.2 );
	o.texCoord1.xy = v.texCoord0.xy * float2( 10, 10.5 ) + _Time.x * SF_Speed * float2( 0.75, 0.5 );
	o.texCoord1.zw = v.texCoord0.xy * float2( 10.6, 10 ) + _Time.x * SF_Speed * float2( 0.5, 0.6 );

	return o;
}

float4 fragClouds_SF( SF_VertexShaderOutput i ) : SV_Target
{
	float3 h0 = tex2D( SF_ScatterMapA, i.texCoord0.xy );
	float3 h1 = tex2D( SF_ScatterMapB, i.texCoord0.zw );
	float3 h2 = tex2D( SF_DensityMap, i.texCoord1.xy );
	float3 h3 = tex2D( SF_AlbedoMap, i.texCoord1.zw );

	float3 fbm = saturate( h0 + h1 + h2 + h3 - SF_Density );

	float alpha = saturate( fbm * SF_AlbedoColor.a * 2 );

#if SF_ALPHATEST_ON

	clip( alpha - SF_AlphaTestValue );

#endif // SF_ALPHATTEST_ON

	float4 diffuseColor = float4( h3, alpha );

	float4 specular = ComputeSpecular( i );
	float3 normal = ComputeNormal( i );
	float3 emissive = ComputeEmissive( i );

	return ComputeLighting( i, diffuseColor, specular, emissive, normal );
}

#endif
