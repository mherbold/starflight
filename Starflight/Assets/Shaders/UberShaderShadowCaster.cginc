
#ifndef UBER_SHADER_SHADOW_CASTER
#define UBER_SHADER_SHADOW_CASTER

#include "UberShaderCore.cginc"

UberShaderVertexOutput vertUberShadowCaster( UberShaderVertexInput v )
{
    UberShaderVertexOutput o;

	o = ComputeVertexShaderOutput( v );

	o.positionClip = UnityApplyLinearShadowBias( UnityClipSpaceShadowCasterPos( v.position, v.normal ) );

	return o;
}

float4 fragUberShadowCaster( float4 svPosition : SV_POSITION, UberShaderVertexOutput i ) : SV_Target
{
#if ALPHA_ON

	float4 diffuseColor = ComputeDiffuseColor( i );

	float alphaRef = tex3D( _DitherMaskLOD, float3( svPosition.xy * 0.25, Alpha * diffuseColor.a * 0.9375 ) ).a;

	clip( alphaRef - 0.01 );

#endif // ALPHA_ON

	return 1;
}

#endif
