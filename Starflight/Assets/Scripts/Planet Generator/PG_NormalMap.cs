
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_NormalMap
{
	readonly int[] c_offsetX = { -1, 0, 1, -1, 1, -1, 0, 1 };
	readonly int[] c_offsetY = { -1, -1, -1, 0, 0, 1, 1, 1 };

	public Color[,] Process( float[,] sourceElevation, float scale, float waterHeight, int reductionScale )
	{
		// UnityEngine.Debug.Log( "*** Normal Map Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var sourceElevationWidth = sourceElevation.GetLength( 1 );
		var sourceElevationHeight = sourceElevation.GetLength( 0 );

		var sourceElevationWidthMask = sourceElevationWidth - 1;

		var outputColorWidth = sourceElevationWidth / reductionScale;
		var outputColorHeight = sourceElevationHeight / reductionScale;

		var outputColor = new Color[ outputColorHeight, outputColorWidth ];

		var half = new Vector3( 0.5f, 0.5f, 0.5f );

		var defaultNormal = new Color( 0.5f, 0.5f, 1.0f );

		var numParallelThreads = 32;
		var rowsPerThread = outputColorHeight / numParallelThreads;

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				if ( ( y < 8 ) || ( y >= ( outputColorHeight - 8 ) ) )
				{
					for ( var x = 0; x < outputColorWidth; x++ )
					{
						outputColor[ y, x ] = defaultNormal;
					}
				}
				else
				{
					var sy = y * reductionScale;

					for ( var x = 0; x < outputColorWidth; x++ )
					{
						var sx = x * reductionScale;

						var centerHeight = sourceElevation[ sy, sx ];

						if ( centerHeight <= waterHeight )
						{
							outputColor[ y, x ] = defaultNormal;
						}
						else
						{
							var normal = Vector3.zero;

							for ( int i = 0; i < 8; i++ )
							{
								var srcX = ( sx + c_offsetX[ i ] + sourceElevationWidth ) & sourceElevationWidthMask;
								var srcY = ( sy + c_offsetY[ i ] );

								var deltaHeight = ( sourceElevation[ srcY, srcX ] - centerHeight ) * scale;

								var vector = new Vector3( c_offsetX[ i ], c_offsetY[ i ], deltaHeight );

								var right = Vector3.Cross( vector, Vector3.forward );

								var up = Vector3.Cross( right, vector );

								normal += Vector3.Normalize( up );
							}

							normal = normal * 0.0625f + half;

							outputColor[ y, x ] = new Color( normal.x, normal.y, normal.z );
						}
					}
				}
			}
		} );

		// UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputColor;
	}
}
