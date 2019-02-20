
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_AlbedoMap
{
	const int c_numRandomPoints = 1 << 8;
	const int c_maxHeight = 1024;

	static Vector2[] m_randomPoints;
	static int[] m_randomNumbers;

	static public void Initialize()
	{
		m_randomPoints = new Vector2[ c_numRandomPoints ];

		for ( var i = 0; i < c_numRandomPoints; i++ )
		{
			m_randomPoints[ i ] = Random.insideUnitCircle;
		}

		m_randomNumbers = new int[ c_maxHeight ];

		for ( var i = 0; i < c_maxHeight; i++ )
		{
			m_randomNumbers[ i ] = Random.Range( 0, c_numRandomPoints );
		}
	}

	public Color[,] Process( float[,] sourceElevation, Color[,] sourceColor, float waterElevation, Color waterColor, Color groundColor )
	{
		// UnityEngine.Debug.Log( "*** Albedo Map Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var outputColorWidth = sourceElevation.GetLength( 1 );
		var outputColorHeight = sourceElevation.GetLength( 0 );

		var outputColor = new Color[ outputColorHeight, outputColorWidth ];

		var sourceColorWidth = sourceColor.GetLength( 1 );
		var sourceColorHeight = sourceColor.GetLength( 0 );

		var xScale = (float) sourceColorWidth / (float) outputColorWidth;
		var yScale = (float) sourceColorHeight / (float) outputColorHeight;

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = outputColorHeight / numParallelThreads;
		var columnsPerThread = outputColorWidth / numParallelThreads;

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				var randomIndex = m_randomNumbers[ y ];

				for ( var x = 0; x < outputColorWidth; x++ )
				{
					var randomPoint = m_randomPoints[ randomIndex ];

					randomIndex = ( randomIndex + 1 ) & ( c_numRandomPoints - 1 );

					var cx = Mathf.FloorToInt( x * xScale + randomPoint.x );
					var cy = Mathf.FloorToInt( y * yScale + randomPoint.y );

					if ( cx < 0 )
					{
						cx += sourceColorWidth;
					}
					else if ( cx >= sourceColorWidth )
					{
						cx -= sourceColorWidth;
					}

					if ( cy < 0 )
					{
						cy = 0;
					}
					else if ( cy >= sourceColorHeight )
					{
						cy = sourceColorHeight - 1;
					}

					var randomColor = sourceColor[ cy, cx ];

					if ( randomColor == waterColor )
					{
						randomColor = groundColor;
					}

					outputColor[ y, x ] = randomColor;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Coloring - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// stopwatch.Restart();

		var xBlurBuffer = new Color[ outputColorHeight, outputColorWidth ];

		var outputColorWidthMask = outputColorWidth - 1;

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				for ( var x1 = 0; x1 < outputColorWidth; x1++ )
				{
					var x0 = ( x1 + outputColorWidth ) & outputColorWidthMask;
					var x2 = ( x1 + 1 ) & outputColorWidthMask;

					xBlurBuffer[ y, x1 ] = outputColor[ y, x0 ] * 0.25f + outputColor[ y, x1 ] * 0.5f + outputColor[ y, x2 ] * 0.25f;
				}
			}
		} );

		var yBlurBuffer = new Color[ outputColorHeight, outputColorWidth ];

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var column = 0; column < columnsPerThread; column++ )
			{
				var x = j * columnsPerThread + column;

				for ( var y1 = 8; y1 < outputColorHeight - 8; y1++ )
				{
					var y0 = y1 - 1;
					var y2 = y1 + 1;

					yBlurBuffer[ y1, x ] = xBlurBuffer[ y0, x ] * 0.25f + outputColor[ y1, x ] * 0.5f + outputColor[ y2, x ] * 0.25f;
				}
			}
		} );

		outputColor = xBlurBuffer;

		// UnityEngine.Debug.Log( "Blur - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// stopwatch.Restart();

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				if ( ( y < 8 ) || ( y >= ( outputColorHeight - 8 ) ) )
				{
					continue;
				}

				for ( var x = 0; x < outputColorWidth; x++ )
				{
					var totalOcclusion = 0.0f;

					for ( var y2 = -3; y2 <= 3; y2++ )
					{
						for ( var x2 = -3; x2 <= 3; x2++ )
						{
							if ( ( x2 == 0 ) && ( y2 == 0 ) )
							{
								continue;
							}

							var hx = ( x + x2 + outputColorWidth ) & outputColorWidthMask;
							var hy = ( y + y2 );

							var s1 = sourceElevation[ hy, hx ] - sourceElevation[ y, x ];

							hx = ( x - x2 + outputColorWidth ) & outputColorWidthMask;
							hy = ( y - y2 );

							var s2 = sourceElevation[ y, x ] - sourceElevation[ hy, hx ];

							var occlusion = s1 - s2;

							if ( occlusion > 0.0f )
							{
								totalOcclusion += occlusion;
							}
						}
					}

					totalOcclusion = Mathf.Max( 0.0f, 1.0f - totalOcclusion * 0.5f );

					outputColor[ y, x ] *= totalOcclusion;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Occlusion - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// stopwatch.Restart();

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				for ( var x = 0; x < outputColorWidth; x++ )
				{
					var height = sourceElevation[ y, x ];

					if ( height <= waterElevation )
					{
						var shallowColor = Color.Lerp( waterColor, outputColor[ y, x ], 0.75f );

						var waterDepth = waterElevation - height;

						outputColor[ y, x ] = Color.Lerp( shallowColor, waterColor, waterDepth * 16.0f );
					}
				}
			}
		} );

		// UnityEngine.Debug.Log( "Water - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputColor;
	}
}
