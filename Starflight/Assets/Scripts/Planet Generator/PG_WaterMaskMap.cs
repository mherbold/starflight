
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_WaterMaskMap
{
	public Color[,] Process( float[,] sourceElevation, float waterHeight, int reductionScale )
	{
		// UnityEngine.Debug.Log( "*** Water Mask Map Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var outputColorWidth = sourceElevation.GetLength( 1 ) / reductionScale;
		var outputColorHeight = sourceElevation.GetLength( 0 ) / reductionScale;

		var outputColor = new Color[ outputColorHeight, outputColorWidth ];

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = outputColorHeight / numParallelThreads;

		Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
			{
				var y = j * rowsPerThread + row;

				var sy = y * reductionScale;

				for ( var x = 0; x < outputColorWidth; x++ )
				{
					var sx = x * reductionScale;

					var height = sourceElevation[ sy, sx ];

					outputColor[ y, x ] = height <= waterHeight ? Color.white : Color.black;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputColor;
	}
}
