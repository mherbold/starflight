
// Modified from https://github.com/keijiro/PerlinNoise/blob/master/Assets/Perlin.cs

using UnityEngine;

public class SeamlessNoise
{
	const int c_permSize = 1 << 15;

	readonly int[,] m_perm;
	
	public SeamlessNoise( int seed, int basePowerOfTwo, int octaves )
	{
		m_perm = new int[ octaves, c_permSize + 1 ];

		for ( var octave = 0; octave < octaves; octave++ )
		{
			for ( var i = 0; i < c_permSize; i++ )
			{
				m_perm[ octave, i ] = i;
			}
		}

		for ( var octave = 0; octave < octaves; octave++ )
		{
			if ( basePowerOfTwo > c_permSize )
			{
				basePowerOfTwo = c_permSize;
			}

			Random.InitState( seed + octave );

			for ( var i = 0; i < c_permSize; i++ )
			{
				int x = Random.Range( i, c_permSize );

				var tmp = m_perm[ octave, i ];

				m_perm[ octave, i ] = m_perm[ octave, x ];
				m_perm[ octave, x ] = tmp;
			}

			m_perm[ octave, basePowerOfTwo ] = m_perm[ octave, 0 ];

			basePowerOfTwo *= 2;
		}
	}

	public float Perlin( int octave, int powerOfTwo, float x, float y )
	{
		if ( powerOfTwo > c_permSize )
		{
			powerOfTwo = c_permSize;
		}

		var iX = Mathf.FloorToInt( x );
		var iY = Mathf.FloorToInt( y );

		var fX = x - iX;
		var fY = y - iY;

		var u = Fade( fX );
		var v = Fade( fY );

		var mask = powerOfTwo - 1;

		iX &= mask;
		iY &= mask;

		var A = ( m_perm[ octave, iX + 0 ] + iY ) & mask;
		var B = ( m_perm[ octave, iX + 1 ] + iY ) & mask;

		var g0 = Grad( fX,     fY,     m_perm[ octave, A ] );
		var g1 = Grad( fX - 1, fY,     m_perm[ octave, B ] );
		var g2 = Grad( fX,     fY - 1, m_perm[ octave, A + 1 ] );
		var g3 = Grad( fX - 1, fY - 1, m_perm[ octave, B + 1 ] );

		var u01 = Lerp( g0, g1, u );
		var u23 = Lerp( g2, g3, u );

		return Lerp( u01, u23, v );
	}

	float Fade( float t )
	{
		return t * t * t * ( t * ( t * 6 - 15 ) + 10 );
	}

	float Lerp( float a, float b, float t )
	{
		return a + t * ( b - a );
	}

	float Grad( float x, float y, int hash )
	{
		return ( ( hash & 1 ) == 0 ? x : -x ) + ( ( hash & 2 ) == 0 ? y : -y );
	}
}
