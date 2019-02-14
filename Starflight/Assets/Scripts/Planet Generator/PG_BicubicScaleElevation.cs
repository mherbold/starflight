
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_BicubicScaleElevation
{
	public float[,] Process( float[,] sourceElevation, int outputElevationWidth, int outputElevationHeight )
	{
		// UnityEngine.Debug.Log( "*** Bicubic Scale Elevation Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var sourceElevationWidth = sourceElevation.GetLength( 1 );
		var sourceElevationHeight = sourceElevation.GetLength( 0 );

		var outputElevation = new float[ outputElevationHeight, outputElevationWidth ];

		var xScale = (float) sourceElevationWidth / (float) outputElevationWidth;
		var yScale = (float) sourceElevationHeight / (float) outputElevationHeight;

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = outputElevationHeight / numParallelThreads;

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				var yt = y * yScale - 0.5f;

				var y1 = Mathf.FloorToInt( yt );
				var y0 = y1 - 1;
				var y2 = y1 + 1;
				var y3 = y1 + 2;

				var dy = yt - y1;

				if ( y0 < 0 )
				{
					y0 = 0;
				}

				if ( y1 < 0 )
				{
					y1 = 0;
				}
				else if ( y1 >= sourceElevationHeight )
				{
					y1 = sourceElevationHeight - 1;
				}

				if ( y2 >= sourceElevationHeight )
				{
					y2 = sourceElevationHeight - 1;
				}

				if ( y3 >= sourceElevationHeight )
				{
					y3 = sourceElevationHeight - 1;
				}

				for ( var x = 0; x < outputElevationWidth; x++ )
				{
					var xt = x * xScale - 0.5f;

					var x1 = Mathf.FloorToInt( xt );
					var x0 = x1 - 1;
					var x2 = x1 + 1;
					var x3 = x1 + 2;

					var dx = xt - x1;

					if ( x0 < 0 )
					{
						x0 += sourceElevationWidth;
					}

					if ( x1 < 0 )
					{
						x1 += sourceElevationWidth;
					}
					else if ( x1 >= sourceElevationWidth )
					{
						x1 -= sourceElevationWidth;
					}

					if ( x2 >= sourceElevationWidth )
					{
						x2 -= sourceElevationWidth;
					}

					if ( x3 >= sourceElevationWidth )
					{
						x3 -= sourceElevationWidth;
					}

					var h00 = sourceElevation[ y0, x0 ];
					var h01 = sourceElevation[ y0, x1 ];
					var h02 = sourceElevation[ y0, x2 ];
					var h03 = sourceElevation[ y0, x3 ];

					var h0 = InterpolateHermite( h00, h01, h02, h03, dx );

					var h10 = sourceElevation[ y1, x0 ];
					var h11 = sourceElevation[ y1, x1 ];
					var h12 = sourceElevation[ y1, x2 ];
					var h13 = sourceElevation[ y1, x3 ];

					var h1 = InterpolateHermite( h10, h11, h12, h13, dx );

					var h20 = sourceElevation[ y2, x0 ];
					var h21 = sourceElevation[ y2, x1 ];
					var h22 = sourceElevation[ y2, x2 ];
					var h23 = sourceElevation[ y2, x3 ];

					var h2 = InterpolateHermite( h20, h21, h22, h23, dx );

					var h30 = sourceElevation[ y3, x0 ];
					var h31 = sourceElevation[ y3, x1 ];
					var h32 = sourceElevation[ y3, x2 ];
					var h33 = sourceElevation[ y3, x3 ];

					var h3 = InterpolateHermite( h30, h31, h32, h33, dx );

					var filteredHeight = InterpolateHermite( h0, h1, h2, h3, dy );

					outputElevation[ y, x ] = filteredHeight;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputElevation;
	}

	float InterpolateHermite( float v0, float v1, float v2, float v3, float t )
	{
		var a = 2.0f * v1;
		var b = v2 - v0;
		var c = 2.0f * v0 - 5.0f * v1 + 4.0f * v2 - v3;
		var d = -v0 + 3.0f * v1 - 3.0f * v2 + v3;

		return 0.5f * ( a + ( b * t ) + ( c * t * t ) + ( d * t * t * t ) );
	}
}
