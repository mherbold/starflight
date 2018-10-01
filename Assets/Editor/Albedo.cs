
using UnityEngine;
using System.Diagnostics;
using System.Threading.Tasks;

public class Albedo
{
	readonly float[,] m_buffer;

	readonly int m_width;
	readonly int m_height;

	public Albedo( float[,] buffer )
	{
		m_buffer = buffer;

		m_width = buffer.GetLength( 1 );
		m_height = buffer.GetLength( 0 );
	}

	public Color[,] Process( int seed, float gain, Color[] legend )
	{
		UnityEngine.Debug.Log( "*** Albedo Process ***" );

		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		// build color map
		var colorMap = new Color[ 256 ];

		colorMap[ 0 ] = legend[ 0 ];

		colorMap[ 0 ].a = 1.0f;

		for ( var i = 1; i < 256; i++ )
		{
			var level = i / 256.0f * ( legend.Length - 1 );

			if ( level < 0.25f )
			{
				colorMap[ i ] = Color.Lerp( legend[ 0 ], legend[ 1 ], level * 4.0f );
			}
			else if ( level < 0.5f )
			{
				colorMap[ i ] = legend[ 1 ];
			}
			else if ( level >= ( legend.Length - 1.5f ) )
			{
				colorMap[ i ] = legend[ legend.Length - 1 ];
			}
			else
			{
				var levelA = Mathf.FloorToInt( level - 0.5f ) + 1;
				var levelB = levelA + 1;

				var colorA = legend[ levelA ];
				var colorB = legend[ levelB ];

				var t = level - ( levelA - 0.5f );

				colorMap[ i ] = Color.Lerp( colorA, colorB, t );
			}

			colorMap[ i ].a = 1.0f;
		}

		// output buffer
		var outputBuffer = new Color[ m_height, m_width ];

		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < m_width; x++ )
			{
				var height = m_buffer[ y, x ] / gain;

				var colorMapIndex = Mathf.FloorToInt( height * 255.0f );

				if ( colorMapIndex < 0 )
				{
					colorMapIndex = 0;
				}
				else if ( colorMapIndex > 255 )
				{
					colorMapIndex = 255;
				}

				// TODO: perhaps lerp between color map cells?

				outputBuffer[ y, x ] = colorMap[ colorMapIndex ];
			}
		} );

		UnityEngine.Debug.Log( "Output Buffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// return the processed buffer
		return outputBuffer;
	}
}
