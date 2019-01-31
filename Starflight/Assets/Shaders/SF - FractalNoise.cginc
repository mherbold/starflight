
#ifndef SF_SHADER_FRACTALNOISE
#define SF_SHADER_FRACTALNOISE

#include "SF - Core.cginc"

float Perlin( float2 texCoord, int octave )
{
	return tex2D( _DetailAlbedoMap, texCoord + 0.3 * octave ) * 2 - 1;
}

float FBM( float2 texCoord, float lacunarity, float persistence, int octaves )
{
	float value = 0;

	for ( int i = 0; i < octaves; i++ )
	{
		value += persistence * Perlin( texCoord, i );

		texCoord *= lacunarity;
	}

	return value;
}

void DoFractalDetails( SF_VertexShaderOutput i, in out float3 albedo, in out float3 specular, in out float3 normal )
{
	float2 texCoord = TRANSFORM_TEX( i.texCoord0, _DetailAlbedoMap );

	float fbm1 = FBM( texCoord, 2, 0.75, 8 );

	albedo.rgb *= fbm1 * 0.4 + 0.6;

//	albedo = 0.5;

	specular.rgb *= saturate( fbm1 * 0.5 + 0.5 );

//	specular = 0.5;

//	float fbm2 = FBM( texCoord + 0.35, 2, 0.75, 4 );
//	float fbm3 = FBM( texCoord + 0.65, 2, 0.75, 4 );

//	normal.xy += float2( fbm2 * 0.1, fbm3 * 0.1 );

//	normalize( normal.xyz );

//	normal.xyz = float3( 0, 1, 0 );
}

#endif
