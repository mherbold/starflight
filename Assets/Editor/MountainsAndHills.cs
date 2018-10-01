
using UnityEngine;
using System.Diagnostics;
using System.Threading.Tasks;

public class MountainsAndHills
{
	Noise m_noise;

	readonly float[,] m_buffer;

	readonly int m_width;
	readonly int m_height;

	int m_octaves;
	int m_scale;
	float m_persistence;

	public MountainsAndHills( float[,] buffer )
	{
		m_buffer = buffer;

		m_width = buffer.GetLength( 1 );
		m_height = buffer.GetLength( 0 );
	}

	public float[,] Process( int seed, float inputGain, int octaves, int mountainScale, int hillScale, float mountainPersistence, float hillPersistence, float mountainPower, float mountainGain, float hillGain )
	{
		UnityEngine.Debug.Log( "*** Mountains and Hills Process ***" );

		var stopwatch = new Stopwatch();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		// setup for mountains
		stopwatch.Start();

		m_noise = new Noise( seed, mountainScale, octaves );

		m_octaves = octaves;
		m_scale = mountainScale;
		m_persistence = mountainPersistence;

		var mountainBuffer = new float[ m_height, m_width ];

		// do mountains
		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < m_width; x++ )
			{
				mountainBuffer[ y, x ] = RidgedTurbulence( x, y );
			}
		} );

		UnityEngine.Debug.Log( "Mountains - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// setup for hills
		m_noise = new Noise( seed, hillScale, octaves );

		m_octaves = octaves;
		m_scale = hillScale;
		m_persistence = hillPersistence;

		var hillBuffer = new float[ m_height, m_width ];

		// do hills
		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < m_width; x++ )
			{
				hillBuffer[ y, x ] = StandardTurbulence( x, y );
			}
		} );

		UnityEngine.Debug.Log( "Hills - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// output buffer
		var outputBuffer = new float[ m_height, m_width ];

		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < m_width; x++ )
			{
				float inputHeight = m_buffer[ y, x ];

				float mountainStart = 0.25f;
				float mountainRange = 0.75f;
				float mountainMultiplier = Mathf.Lerp( 0.0f, 1.0f, ( m_buffer[ y, x ] - mountainStart ) / mountainRange );

				float hillCenter = 0.50f;
				float hillRange = 0.45f;
				float hillMultiplier = Mathf.Lerp( 1.0f, 0.0f, Mathf.Abs( hillCenter - m_buffer[ y, x ] ) / hillRange );

				float p1 = m_height / (float) 10.0f * 0.1f;
				float p2 = m_height / (float) 10.0f * 3.0f;
				float p3 = m_height / (float) 10.0f * 7.0f;
				float p4 = m_height / (float) 10.0f * 9.9f;

				float poleMultiplier = Mathf.Lerp( 0.0f, 1.0f, ( y - p1 ) / ( p2 - p1 ) ) * Mathf.Lerp( 1.0f, 0.0f, ( y - p3 ) / ( p4 - p3 ) );

				outputBuffer[ y, x ] = inputGain * inputHeight + poleMultiplier * ( mountainGain * mountainMultiplier * ( inputHeight * Mathf.Pow( mountainBuffer[ y, x ], mountainPower ) ) + hillGain * hillMultiplier * hillBuffer[ y, x ] );
			}
		} );

		UnityEngine.Debug.Log( "Output Buffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// return the processed buffer
		return outputBuffer;
	}

	float StandardTurbulence( float x, float y )
	{
		int scale = m_scale;

		float amplitude = 1.0f;
		float sum = 0;

		for ( var i = 0; i < m_octaves; i++ )
		{
			float coordinateScale = (float) scale / (float) m_width;

			sum += amplitude * m_noise.Perlin( i, scale, x * coordinateScale, y * coordinateScale );

			amplitude *= m_persistence;
			scale *= 2;
		}

		return sum * 0.5f + 0.5f;
	}

	float BillowedTurbulence( float x, float y )
	{
		int scale = m_scale;

		float amplitude = 1.0f;
		float sum = 0;

		for ( var i = 0; i < m_octaves; i++ )
		{
			float coordinateScale = (float) scale / (float) m_width;

			sum += amplitude * Mathf.Abs( m_noise.Perlin( i, scale, x * coordinateScale, y * coordinateScale ) );

			amplitude *= m_persistence;
			scale *= 2;
		}

		return sum * 2.0f;
	}

	float RidgedTurbulence( float x, float y )
	{
		int scale = m_scale;

		float amplitude = 1.0f;
		float sum = 0;

		for ( var i = 0; i < m_octaves; i++ )
		{
			float coordinateScale = (float) scale / (float) m_width;

			sum += amplitude * ( 1.0f - Mathf.Abs( m_noise.Perlin( i, scale, x * coordinateScale, y * coordinateScale ) ) );

			amplitude *= m_persistence;
			scale *= 2;
		}

		return sum;
	}
}
