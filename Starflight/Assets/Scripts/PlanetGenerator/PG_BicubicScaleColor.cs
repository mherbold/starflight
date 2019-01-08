
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_BicubicScaleColor
{
	public Color[,] Process( Color[,] sourceColor, int outputColorWidth, int outputColorHeight )
	{
		// UnityEngine.Debug.Log( "*** Bicubic Scale Color Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var sourceColorWidth = sourceColor.GetLength( 1 );
		var sourceColorHeight = sourceColor.GetLength( 0 );

		var outputColor = new Color[ outputColorHeight, outputColorWidth ];

		var xScale = (float) sourceColorWidth / (float) outputColorWidth;
		var yScale = (float) sourceColorHeight / (float) outputColorHeight;

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = outputColorHeight / numParallelThreads;

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
				else if ( y1 >= sourceColorHeight )
				{
					y1 = sourceColorHeight - 1;
				}

				if ( y2 >= sourceColorHeight )
				{
					y2 = sourceColorHeight - 1;
				}

				if ( y3 >= sourceColorHeight )
				{
					y3 = sourceColorHeight - 1;
				}

				for ( var x = 0; x < outputColorWidth; x++ )
				{
					var xt = x * xScale - 0.5f;

					var x1 = Mathf.FloorToInt( xt );
					var x0 = x1 - 1;
					var x2 = x1 + 1;
					var x3 = x1 + 2;

					var dx = xt - x1;

					if ( x0 < 0 )
					{
						x0 += sourceColorWidth;
					}

					if ( x1 < 0 )
					{
						x1 += sourceColorWidth;
					}
					else if ( x1 >= sourceColorWidth )
					{
						x1 -= sourceColorWidth;
					}

					if ( x2 >= sourceColorWidth )
					{
						x2 -= sourceColorWidth;
					}

					if ( x3 >= sourceColorWidth )
					{
						x3 -= sourceColorWidth;
					}

					var h00 = sourceColor[ y0, x0 ];
					var h01 = sourceColor[ y0, x1 ];
					var h02 = sourceColor[ y0, x2 ];
					var h03 = sourceColor[ y0, x3 ];

					var h0 = InterpolateColor( h00, h01, h02, h03, dx );

					var h10 = sourceColor[ y1, x0 ];
					var h11 = sourceColor[ y1, x1 ];
					var h12 = sourceColor[ y1, x2 ];
					var h13 = sourceColor[ y1, x3 ];

					var h1 = InterpolateColor( h10, h11, h12, h13, dx );

					var h20 = sourceColor[ y2, x0 ];
					var h21 = sourceColor[ y2, x1 ];
					var h22 = sourceColor[ y2, x2 ];
					var h23 = sourceColor[ y2, x3 ];

					var h2 = InterpolateColor( h20, h21, h22, h23, dx );

					var h30 = sourceColor[ y3, x0 ];
					var h31 = sourceColor[ y3, x1 ];
					var h32 = sourceColor[ y3, x2 ];
					var h33 = sourceColor[ y3, x3 ];

					var h3 = InterpolateColor( h30, h31, h32, h33, dx );

					var filteredHeight = InterpolateColor( h0, h1, h2, h3, dy );

					outputColor[ y, x ] = filteredHeight;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputColor;
	}

	Color InterpolateColor( Color v0, Color v1, Color v2, Color v3, float t )
	{
		var r = InterpolateHermite( v0.r, v1.r, v2.r, v3.r, t );
		var g = InterpolateHermite( v0.g, v1.g, v2.g, v3.g, t );
		var b = InterpolateHermite( v0.b, v1.b, v2.b, v3.b, t );

		return new Color( r, g, b );
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
