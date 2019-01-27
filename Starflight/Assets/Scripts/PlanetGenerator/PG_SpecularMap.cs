
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_SpecularMap
{
	const float c_groundSpecularPowerAtWaterLevel = 0.1f;
	const float c_groundSpecularPowerAtSnowLevel = 0.75f;

	public Color[,] Process( float[,] sourceElevation, Color[,] albedoMap, float waterHeight, Color waterSpecularColor, float waterSpecularPower, int reductionScale )
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

					Color specular;

					if ( height < waterHeight )
					{
						// desaturate terrain specular color by 75%
						specular = Color.Lerp( color, new Color( v, v, v ), 0.75f );

						// tint desaturated terrain specular color towards deep water specular color by 25%
						var shallowColor = Color.Lerp( specular, waterSpecularColor, 0.25f );

						// calculate how deep the water is at this point
						var heightBelowWater = waterHeight - height;

						// blend from shallow water specular color towards deep water specular color based on water depth
						specular = Color.Lerp( shallowColor, waterSpecularColor, heightBelowWater * 16.0f );

						// start with ground specular power blended towards deep water specular power by 25%
						specular.a = Mathf.Lerp( c_groundSpecularPowerAtWaterLevel, waterSpecularPower, 0.25f );

						// blend from shallow water specular power towards deep water specular power based on water depth
						specular.a = Mathf.Lerp( specular.a, waterSpecularPower, heightBelowWater * 16.0f );
					}
					else
					{
						var heightAboveWater = height - waterHeight;
						var maximumHeightAboveWater = 2.0f - waterHeight;

						// desaturate terrain specular color by 50%
						specular = Color.Lerp( color, new Color( v, v, v ), 0.5f );

						// blend from water level specular power towards snow level specular power based on height above water
						specular.a = Mathf.Lerp( c_groundSpecularPowerAtWaterLevel, c_groundSpecularPowerAtSnowLevel, heightAboveWater / maximumHeightAboveWater );
					}

					outputColor[ y, x ] = specular;
				}
			}
		} );

		// UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputColor;
	}
}
