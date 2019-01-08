
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_Mountains
{
	SeamlessNoise m_noise;

	int m_octaves;
	int m_scale;
	float m_persistence;

	int m_outputElevationWidth;
	int m_outputElevationHeight;

	public float[,] Process( float[,] sourceElevation, int seed, int octaves, int mountainScale, float mountainPersistence, float mountainGain, float waterHeight )
	{
		UnityEngine.Debug.Log( "*** Mountains Process ***" );

		var stopwatch = new Stopwatch();

		stopwatch.Start();

		m_outputElevationWidth = sourceElevation.GetLength( 1 );
		m_outputElevationHeight = sourceElevation.GetLength( 0 );

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = m_outputElevationHeight / numParallelThreads;

		var mountainStart = waterHeight;
		var mountainRange = 1.0f - mountainStart;

		m_noise = new SeamlessNoise( seed, mountainScale, octaves );

		m_octaves = octaves;
		m_scale = mountainScale;
		m_persistence = mountainPersistence;

		UnityEngine.Debug.Log( "SeamlessNoise Init - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		var mountainBuffer = new float[ m_outputElevationHeight, m_outputElevationWidth ];

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				for ( var x = 0; x < m_outputElevationWidth; x++ )
				{
					mountainBuffer[ y, x ] = RidgedTurbulence( x, y );
				}
			}
		} );

		UnityEngine.Debug.Log( "Mountains - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		var outputBuffer = new float[ m_outputElevationHeight, m_outputElevationWidth ];

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				var p1 = m_outputElevationHeight * 0.1f;
				var p2 = m_outputElevationHeight * 0.2f;
				var p3 = m_outputElevationHeight * 0.8f;
				var p4 = m_outputElevationHeight * 0.9f;

				var poleMultiplier = Mathf.SmoothStep( 0.0f, 1.0f, ( y - p1 ) / ( p2 - p1 ) ) * Mathf.SmoothStep( 1.0f, 0.0f, ( y - p3 ) / ( p4 - p3 ) );

				for ( var x = 0; x < m_outputElevationWidth; x++ )
				{
					var inputHeight = sourceElevation[ y, x ];

					var mountainMultiplier = Mathf.Lerp( 0.0f, 1.0f, ( sourceElevation[ y, x ] - mountainStart ) / mountainRange );
					var mountainElevation = mountainGain * Mathf.Pow( mountainMultiplier * mountainBuffer[ y, x ], 2.0f );

					outputBuffer[ y, x ] = inputHeight + poleMultiplier * mountainElevation;
				}
			}
		} );

		UnityEngine.Debug.Log( "Output - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputBuffer;
	}

	float StandardTurbulence( float x, float y )
	{
		var scale = m_scale;
		var amplitude = 1.0f;
		var sum = 0.0f;

		for ( var i = 0; i < m_octaves; i++ )
		{
			var coordinateScale = (float) scale / (float) m_outputElevationWidth;

			sum += amplitude * m_noise.Perlin( i, scale, x * coordinateScale, y * coordinateScale );

			amplitude *= m_persistence;
			scale *= 2;
		}

		return sum * 0.5f + 0.5f;
	}

	float BillowedTurbulence( float x, float y )
	{
		var scale = m_scale;
		var amplitude = 1.0f;
		var sum = 0.0f;

		for ( var i = 0; i < m_octaves; i++ )
		{
			var coordinateScale = (float) scale / (float) m_outputElevationWidth;

			sum += amplitude * Mathf.Abs( m_noise.Perlin( i, scale, x * coordinateScale, y * coordinateScale ) );

			amplitude *= m_persistence;
			scale *= 2;
		}

		return sum * 2.0f;
	}

	float RidgedTurbulence( float x, float y )
	{
		var scale = m_scale;
		var amplitude = 1.0f;
		var sum = 0.0f;

		for ( var i = 0; i < m_octaves; i++ )
		{
			var coordinateScale = (float) scale / (float) m_outputElevationWidth;

			sum += amplitude * ( 1.0f - Mathf.Abs( m_noise.Perlin( i, scale, x * coordinateScale, y * coordinateScale ) ) );

			amplitude *= m_persistence;
			scale *= 2;
		}

		return sum;
	}
}
