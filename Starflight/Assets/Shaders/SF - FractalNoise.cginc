
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

float GetFractalNoise( SF_VertexShaderOutput i )
{
	float2 texCoord = TRANSFORM_TEX( i.texCoord0, _DetailAlbedoMap );

	return FBM( texCoord, 2, 0.75, 8 );
}

#endif
