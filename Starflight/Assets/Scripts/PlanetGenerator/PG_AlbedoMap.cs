
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_AlbedoMap
{
	public Color[,] Process( float[,] sourceElevation, Color[,] sourceColor, float waterHeight, Color waterColor, Color groundColor )
	{
		// UnityEngine.Debug.Log( "*** Albedo Map Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var outputElevationWidth = sourceElevation.GetLength( 1 );
		var outputElevationHeight = sourceElevation.GetLength( 0 );

		var outputElevation = new Color[ outputElevationHeight, outputElevationWidth ];

		var sourceColorWidth = sourceColor.GetLength( 1 );
		var sourceColorHeight = sourceColor.GetLength( 0 );

		var xScale = (float) sourceColorWidth / (float) outputElevationWidth;
		var yScale = (float) sourceColorHeight / (float) outputElevationHeight;

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = outputElevationHeight / numParallelThreads;
		var columnsPerThread = outputElevationWidth / numParallelThreads;

		var numRandomPoints = 1 << 8;

		var randomPoints = new Vector2[ numRandomPoints ];

		for ( var i = 0; i < numRandomPoints; i++ )
		{
			randomPoints[ i ] = Random.insideUnitCircle;
		}

		var randomNumbers = new int[ outputElevationHeight ];

		for ( var i = 0; i < outputElevationHeight; i++ )
		{
			randomNumbers[ i ] = Random.Range( 0, numRandomPoints );
		}

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				var randomIndex = randomNumbers[ y ];

				for ( var x = 0; x < outputElevationWidth; x++ )
				{
					var randomPoint = randomPoints[ randomIndex ];

					randomIndex = ( randomIndex + 1 ) & ( numRandomPoints - 1 );

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

					outputElevation[ y, x ] = randomColor;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Coloring - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// stopwatch.Restart();

		var xBlurBuffer = new Color[ outputElevationHeight, outputElevationWidth ];

		var m = outputElevationWidth - 1;

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				for ( var x1 = 0; x1 < outputElevationWidth; x1++ )
				{
					var x0 = ( x1 + m ) & m;
					var x2 = ( x1 + 1 ) & m;

					xBlurBuffer[ y, x1 ] = outputElevation[ y, x0 ] * 0.25f + outputElevation[ y, x1 ] * 0.5f + outputElevation[ y, x2 ] * 0.25f;
				}
			}
		} );

		var yBlurBuffer = new Color[ outputElevationHeight, outputElevationWidth ];

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var column = 0; column < columnsPerThread; column++ )
			{
				var x = j * columnsPerThread + column;

				for ( var y1 = 8; y1 < outputElevationHeight - 8; y1++ )
				{
					var y0 = y1 - 1;
					var y2 = y1 + 1;

					yBlurBuffer[ y1, x ] = xBlurBuffer[ y0, x ] * 0.25f + outputElevation[ y1, x ] * 0.5f + outputElevation[ y2, x ] * 0.25f;
				}
			}
		} );

		outputElevation = xBlurBuffer;

		// UnityEngine.Debug.Log( "Blur - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// stopwatch.Restart();

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				if ( ( y < 8 ) || ( y >= ( outputElevationHeight - 8 ) ) )
				{
					continue;
				}

				for ( var x = 0; x < outputElevationWidth; x++ )
				{
					var totalOcclusion = 0.0f;

					for ( var y2 = -2; y2 <= 2; y2++ )
					{
						for ( var x2 = -2; x2 <= 2; x2++ )
						{
							if ( ( x2 == 0 ) && ( y2 == 0 ) )
							{
								continue;
							}

							var hx = ( x + x2 + m ) & m;
							var hy = ( y + y2 );

							var s1 = sourceElevation[ hy, hx ] - sourceElevation[ y, x ];

							hx = ( x - x2 + m ) & m;
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

					outputElevation[ y, x ] *= totalOcclusion;
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

				for ( var x = 0; x < outputElevationWidth; x++ )
				{
					var height = sourceElevation[ y, x ];

					if ( height <= waterHeight )
					{
						var shallowColor = Color.Lerp( waterColor, outputElevation[ y, x ], 0.75f );

						var waterDepth = waterHeight - height;

						outputElevation[ y, x ] = Color.Lerp( shallowColor, waterColor, waterDepth * 16.0f );
					}
				}
			}
		} );

		// UnityEngine.Debug.Log( "Water - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputElevation;
	}
}
