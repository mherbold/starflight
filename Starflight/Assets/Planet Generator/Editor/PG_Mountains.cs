
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_Mountains
{
	SimplexNoise m_noise;

	int m_octaves;

	float m_scale;
	float m_lacunarity;
	float m_persistence;

	float m_xOffset;
	float m_yOffset;

	int m_outputElevationWidth;
	int m_outputElevationHeight;

	public float[,] Process( float[,] sourceElevation, int planetId, int octaves, float mountainScale, float mountainLacunarity, float mountainPersistence, float mountainGain, float waterElevation )
	{
		// UnityEngine.Debug.Log( "*** Mountains Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		m_outputElevationWidth = sourceElevation.GetLength( 1 );
		m_outputElevationHeight = sourceElevation.GetLength( 0 );

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = m_outputElevationHeight / numParallelThreads;

		var mountainStart = waterElevation;
		var mountainRange = 1.0f - mountainStart;

		m_noise = new SimplexNoise();

		m_octaves = octaves;
		m_scale = mountainScale;
		m_lacunarity = mountainLacunarity;
		m_persistence = mountainPersistence;

		m_xOffset = planetId / 29.0f;
		m_yOffset = planetId % 29.0f;

		var mountainBuffer = new float[ m_outputElevationHeight, m_outputElevationWidth ];

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				for ( var x = 0; x < m_outputElevationWidth; x++ )
				{
					mountainBuffer[ y, x ] = BillowedTurbulence( x, y ) * mountainGain - ( mountainGain * 0.5f );
				}
			}
		} );

		// PG_Tools.SaveAsPNG( mountainBuffer, Application.dataPath + "/Exported/" + "Debug - Mountain Buffer.png" );

		// UnityEngine.Debug.Log( "Mountains - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// stopwatch.Restart();

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
					var mountainElevation = mountainBuffer[ y, x ] * mountainMultiplier;

					outputBuffer[ y, x ] = inputHeight - poleMultiplier * mountainElevation;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Output - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputBuffer;
	}

	float StandardTurbulence( float x, float y )
	{
		var scale = m_scale;
		var amplitude = 1.0f;
		var sum = 0.0f;

		for ( var i = 0; i < m_octaves; i++ )
		{
			var period = (float) scale * (float) m_outputElevationWidth;

			sum += amplitude * m_noise.GenerateSeamlessX( x * scale + m_xOffset, y * scale + m_yOffset, period );

			scale *= m_lacunarity;
			amplitude *= m_persistence;
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
			var period = (float) scale * (float) m_outputElevationWidth;

			sum += amplitude * Mathf.Abs( m_noise.GenerateSeamlessX( x * scale + m_xOffset, y * scale + m_yOffset, period ) );

			scale *= m_lacunarity;
			amplitude *= m_persistence;
		}

		return sum;
	}

	float RidgedTurbulence( float x, float y )
	{
		var scale = m_scale;
		var amplitude = 1.0f;
		var sum = 0.0f;

		for ( var i = 0; i < m_octaves; i++ )
		{
			var period = (float) scale * (float) m_outputElevationWidth;

			sum += amplitude * ( 1.0f - Mathf.Abs( m_noise.GenerateSeamlessX( x * scale + m_xOffset, y * scale + m_yOffset, period ) ) );

			scale *= m_lacunarity;
			amplitude *= m_persistence;
		}

		return sum;
	}
}
