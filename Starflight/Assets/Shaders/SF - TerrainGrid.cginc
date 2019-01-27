
#ifndef SF_TERRAIN_GRID
#define SF_TERRAIN_GRID

#include "SF - Core.cginc"

SF_VertexShaderOutput vertTerrainGrid_SF( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	float4 positionWorld = mul( unity_ObjectToWorld, v.position );
	float3 normalWorld = normalize( mul( v.normal, (float3x3) unity_WorldToObject ) );

	float2 texCoord = TRANSFORM_TEX( positionWorld.xz, SF_ElevationMap );

	float elevation = tex2Dlod( SF_ElevationMap, float4( texCoord, 0, 0 ) ) * SF_ElevationScale;

	positionWorld.y += elevation;

	o.positionClip = mul( UNITY_MATRIX_VP, positionWorld );
	o.color = v.color;
	o.texCoord0 = float4( texCoord, 0, 1 );
	o.texCoord1 = float4( v.texCoord1, 0, 1 );
	o.positionWorld = positionWorld;
	o.eyeDir = normalize( positionWorld.xyz - _WorldSpaceCameraPos );
	o.normalWorld = normalWorld;

	#if SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON

		float3 tangentWorld = normalize( mul( v.tangent.xyz, (float3x3) unity_WorldToObject ) );
		float3 binormalWorld = cross( normalWorld, tangentWorld ) * v.tangent.w * unity_WorldTransformParams.w;

		o.tangentWorld = tangentWorld;
		o.binormalWorld = binormalWorld;

	#endif // SF_NORMALMAP_ON || SF_DETAILNORMALMAP_ON
	
	#if SF_BEHINDEVERYTHING_ON

		o.texCoord1 = ComputeScreenPos( o.positionClip );

	#endif // SF_BEHINDEVERYTHING_ON

	return o;
}

#endif
