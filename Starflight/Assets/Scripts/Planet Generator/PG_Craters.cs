
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_Craters
{
	static float[][,] m_craterTextureMaps;

	static public void Initialize()
	{
		// load crater texture maps
		m_craterTextureMaps = new float[ 3 ][,];

		for ( var i = 0; i < m_craterTextureMaps.Length; i++ )
		{
			var texture = Resources.Load<Texture2D>( "Craters " + ( i + 1 ) );

			m_craterTextureMaps[ i ] = new float[ texture.height, texture.width ];

			for ( var y = 0; y < texture.height; y++ )
			{
				for ( var x = 0; x < texture.width; x++ )
				{
					var color = texture.GetPixel( x, y );

					m_craterTextureMaps[ i ][ y, x ] = color.r;
				}
			}
		}
	}

	public float[,] Process( float[,] sourceElevation, int planetId, float craterGain, float waterElevation )
	{
		 UnityEngine.Debug.Log( "*** Craters Process ***" );

		 var stopwatch = new Stopwatch();

		 stopwatch.Start();

		var outputElevationWidth = sourceElevation.GetLength( 1 );
		var outputElevationHeight = sourceElevation.GetLength( 0 );

		var outputElevation = new float[ outputElevationHeight, outputElevationWidth ];

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var numParallelThreads = 32;

        var rowsPerThread = outputElevationHeight / numParallelThreads;

		var craterStart = waterElevation;
		var craterRange = 1.0f - craterStart;

        var craterLen = m_craterTextureMaps.Length;
        var texture = m_craterTextureMaps[ planetId % craterLen ];

        var flipHorizontal = planetId % ( craterLen * 2 ) >= craterLen;
        var flipVertical = planetId % ( craterLen * 4 ) >= craterLen * 2;

        // Well flip half of the crater textures horizontally, and half of them vertically, which will give us more variation
        var startingY = flipVertical ? outputElevationHeight -1 : 0;
        var startingX = flipHorizontal ? outputElevationWidth -1 : 0;

        Parallel.For( 0, numParallelThreads, parallelOptions, j =>
		{
			for ( var row = 0; row < rowsPerThread; row++ )
            {
                var y = j * rowsPerThread + row;
                var craterY = Mathf.Abs( startingY - y );

				for ( var x = 0; x < outputElevationWidth; x++ )
                { 
                    var craterX = Mathf.Abs( startingX - x);
					var craterMultiplier = Mathf.Sqrt( Mathf.Lerp( 0.0f, 1.0f, ( sourceElevation[ y, x ] - craterStart ) / craterRange ) );
					outputElevation[ y, x ] = sourceElevation[ y, x ] + texture[craterY, craterX] * craterMultiplier * craterGain;
				}
			}
		} );

		UnityEngine.Debug.Log( "Output - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputElevation;
	}
}
