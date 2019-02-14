
#ifndef SF_SHADER_TERRAIN_GRID
#define SF_SHADER_TERRAIN_GRID

#include "SF - Core.cginc"

float4 getTerrainGridWorldCoordinates( SF_VertexShaderInput v, out float2 texCoord )
{
	float4 positionWorld = mul( unity_ObjectToWorld, v.position );

	texCoord = TRANSFORM_TEX( positionWorld.xz, SF_ElevationMap );

	float elevation = tex2Dlod( SF_ElevationMap, float4( texCoord, 0, 0 ) ) * SF_ElevationScale;

	positionWorld.y += elevation;

	return positionWorld;
}

SF_VertexShaderOutput vertTerrainGrid_SF( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	float2 texCoord;

	float4 positionWorld = getTerrainGridWorldCoordinates( v, texCoord );
	float3 normalWorld = float3( 0, 1, 0 );

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

SF_VertexShaderOutput vertTerrainGridShadowCaster_SF( SF_VertexShaderInput v )
{
	SF_VertexShaderOutput o;

	o = vertTerrainGrid_SF( v );

	if ( unity_LightShadowBias.z != 0 )
	{
		float3 wLight = normalize( UnityWorldSpaceLightDir( o.positionWorld.xyz ) );

		float shadowCos = dot( o.normalWorld, wLight );

		float shadowSine = sqrt( 1 - shadowCos * shadowCos );

		float normalBias = unity_LightShadowBias.z * shadowSine;

		o.positionWorld.xyz -= o.normalWorld * normalBias;
	}

	o.positionClip = UnityApplyLinearShadowBias( mul( UNITY_MATRIX_VP, o.positionWorld ) );

	return o;
}

#endif
