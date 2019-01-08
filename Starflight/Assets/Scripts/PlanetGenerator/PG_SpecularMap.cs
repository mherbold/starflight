
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_SpecularMap
{
	public Color[,] Process( float[,] sourceElevation, Color[,] albedoMap, float waterHeight, int reductionScale )
	{
		// UnityEngine.Debug.Log( "*** Specular Map Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var outputColorWidth = sourceElevation.GetLength( 1 ) / reductionScale;
		var outputColorHeight = sourceElevation.GetLength( 0 ) / reductionScale;

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;
		var rowsPerThread = outputColorHeight / numParallelThreads;

		var outputColor = new Color[ outputColorHeight, outputColorWidth ];

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
					var color = albedoMap[ sy, sx ];

					Color.RGBToHSV( color, out float h, out float s, out float v );

					Color specular = Color.Lerp( color, new Color( v, v, v ), 0.75f );

					specular.a = height * 0.5f;

					if ( height < waterHeight )
					{
						var shallowColor = Color.Lerp( specular, Color.white, 0.25f );

						var waterDepth = waterHeight - height;

						specular = Color.Lerp( shallowColor, Color.white, waterDepth * 16.0f );

						specular.a = Mathf.Lerp( specular.a, 0.75f, 0.25f );
						specular.a = Mathf.Lerp( specular.a, 0.75f, waterDepth * 16.0f );
					}
					else
					{
						specular = Color.Lerp( color, new Color( v, v, v ), 0.5f );

						specular.a = height * 0.5f;
					}

					outputColor[ y, x ] = specular;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputColor;
	}
}
