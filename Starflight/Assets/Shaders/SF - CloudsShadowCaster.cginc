
#ifndef SF_SHADER_CLOUDS_SHADOW_CASTER
#define SF_SHADER_CLOUDS_SHADOW_CASTER

#include "SF - Core.cginc"

SF_VertexShaderOutput vertCloudsShadowCaster_SF( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	o = ComputeVertexShaderOutput( v );

	o.positionClip = UnityApplyLinearShadowBias( UnityClipSpaceShadowCasterPos( v.position, v.normal ) );

	float2 baseTexCoord = o.texCoord0.xy;

	o.texCoord0.xy = baseTexCoord * float2( 10.6, 10 ) + _Time.x * SF_Speed * float2( 0.5, 0.6 ) * SF_BaseScaleOffset.xy;
	o.texCoord0.zw = baseTexCoord * float2( 10, 10.5 ) + _Time.x * SF_Speed * float2( 0.75, 0.5 ) * SF_BaseScaleOffset.xy;
	o.texCoord1.xy = baseTexCoord * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1.5, 1 ) * SF_BaseScaleOffset.xy;
	o.texCoord1.zw = baseTexCoord * float2( 2, 2 ) + _Time.x * SF_Speed * float2( 1, 1.2 ) * SF_BaseScaleOffset.xy;

	return o;
}

float4 fragCloudsShadowCaster_SF( float4 svPosition : SV_POSITION, SF_VertexShaderOutput i ) : SV_Target
{
	#if SF_ALPHA_ON

		float3 h0 = tex2D( SF_AlbedoMap, i.texCoord0.xy );
		float3 h1 = tex2D( SF_DensityMap, i.texCoord0.zw );
		float3 h2 = tex2D( SF_ScatterMapA, i.texCoord1.xy );
		float3 h3 = tex2D( SF_ScatterMapB, i.texCoord1.zw );

		float3 fbm = saturate( h0 + h1 + h2 + h3 - SF_Density );

		float alpha = saturate( fbm * SF_AlbedoColor.a * 2 );

		float4 diffuseColor = ComputeDiffuseColor( i );

		diffuseColor.a *= alpha;

		#if SF_ALPHATEST_ON

			clip( diffuseColor.a - SF_AlphaTestValue );

		#endif // SF_ALPHATTEST_ON

		float alphaRef = tex3D( _DitherMaskLOD, float3( svPosition.xy * 0.25, diffuseColor.a * 0.9375 ) ).a;

		clip( alphaRef - 0.01 );

	#endif // SF_ALPHA_ON

	return 1;
}

#endif
