
// From https://github.com/keijiro/PerlinNoise/blob/master/Assets/Perlin.cs

using UnityEngine;

public class Noise
{
	const int c_permSize = 32768;

	readonly int[,] m_perm;
	
	public Noise( int seed, int basePowerOfTwo, int octaves )
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

		int mask = powerOfTwo - 1;

		var X = Mathf.FloorToInt( x ) & mask;
		var Y = Mathf.FloorToInt( y ) & mask;

		x -= Mathf.Floor( x );
		y -= Mathf.Floor( y );

		var u = Fade( x );
		var v = Fade( y );

		var A = ( m_perm[ octave, X + 0 ] + Y ) & mask;
		var B = ( m_perm[ octave, X + 1 ] + Y ) & mask;

		return Lerp( v, Lerp( u, Grad( m_perm[ octave, A ], x, y ), Grad( m_perm[ octave, B ], x - 1, y ) ), Lerp( u, Grad( m_perm[ octave, A + 1 ], x, y - 1 ), Grad( m_perm[ octave, B + 1 ], x - 1, y - 1 ) ) );
	}

	float Fade( float t )
	{
		return t * t * t * ( t * ( t * 6 - 15 ) + 10 );
	}

	float Lerp( float t, float a, float b )
	{
		return a + t * ( b - a );
	}

	float Grad( int hash, float x, float y )
	{
		return ( ( hash & 1 ) == 0 ? x : -x ) + ( ( hash & 2 ) == 0 ? y : -y );
	}
}
