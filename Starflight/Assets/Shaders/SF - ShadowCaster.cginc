
#ifndef SF_SHADER_SHADOW_CASTER
#define SF_SHADER_SHADOW_CASTER

#include "SF - Core.cginc"

SF_VertexShaderOutput vertShadowCaster_SF( SF_VertexShaderInput v )
{
    SF_VertexShaderOutput o;

	o = ComputeVertexShaderOutput( v );

	o.positionClip = UnityApplyLinearShadowBias( UnityClipSpaceShadowCasterPos( v.position, v.normal ) );

	return o;
}

float4 fragShadowCaster_SF( float4 svPosition : SV_POSITION, SF_VertexShaderOutput i ) : SV_Target
{
#if SF_ALPHA_ON

	float4 diffuseColor = ComputeDiffuseColor( i );

#if SF_ALPHATEST_ON

	clip( diffuseColor.a - SF_AlphaTestValue );

#endif // SF_ALPHATTEST_ON

	float alphaRef = tex3D( _DitherMaskLOD, float3( svPosition.xy * 0.25, diffuseColor.a * 0.9375 ) ).a;

	clip( alphaRef - 0.01 );

#endif // SF_ALPHA_ON

	return 1;
}

#endif
