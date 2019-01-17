
#ifndef SF_SHADER_CLOUDS
#define SF_SHADER_CLOUDS

#define SF_IS_FORWARD 1

#include "SF - Core.cginc"

SF_VertexShaderOutput vertClouds_SF( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o = ComputeVertexShaderOutput( v );

	float2 baseTexCoord = o.texCoord0.xy;

	o.texCoord0.xy = baseTexCoord * float2( 10.6, 10 ) + _Time.x * SF_Speed * float2( 0.5, 0.6 ) * SF_BaseScaleOffset.xy;
	o.texCoord0.zw = baseTexCoord * float2( 10, 10.5 ) + _Time.x * SF_Speed * float2( 0.75, 0.5 ) * SF_BaseScaleOffset.xy;
	o.texCoord1.xy = baseTexCoord * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1.5, 1 ) * SF_BaseScaleOffset.xy;
	o.texCoord1.zw = baseTexCoord * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1, 1.2 ) * SF_BaseScaleOffset.xy;

	return o;
}

float4 fragClouds_SF( SF_VertexShaderOutput i ) : SV_Target
{
	float3 h0 = tex2D( SF_AlbedoMap, i.texCoord0.xy );
	float3 h1 = tex2D( SF_DensityMap, i.texCoord0.zw );
	float3 h2 = tex2D( SF_ScatterMapA, i.texCoord1.xy );
	float3 h3 = tex2D( SF_ScatterMapB, i.texCoord1.zw );

	float3 fbm = saturate( h0 + h1 + h2 + h3 - SF_Density );

	float alpha = saturate( fbm * SF_AlbedoColor.a * 2 );

	float4 diffuseColor = ComputeDiffuseColor( i );
	float4 specular = ComputeSpecular( i );
	float3 normal = ComputeNormal( i );
	float3 emissive = ComputeEmissive( i );

	diffuseColor.a *= alpha;

#if SF_ALPHATEST_ON

	clip( diffuseColor.a - SF_AlphaTestValue );

#endif // SF_ALPHATTEST_ON

	return ComputeLighting( i, diffuseColor, specular, emissive, normal );
}

#endif
