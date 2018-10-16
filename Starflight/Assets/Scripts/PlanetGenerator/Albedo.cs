
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

	public Color[,] Process( Color[] legend )
	{
		UnityEngine.Debug.Log( "*** Albedo Process ***" );

		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		// for blending water
		var shoreWaterColor = legend[ 0 ] * legend[ 1 ];

		// max elevation
		var maxElevation = legend[ legend.Length - 1 ].a;

		// output buffer
		var outputBuffer = new Color[ m_height, m_width ];

		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < m_width; x++ )
			{
				var level = m_buffer[ y, x ];

				if ( level < 0.25f )
				{
					outputBuffer[ y, x ] = Color.Lerp( legend[ 0 ], shoreWaterColor, 4.0f * level );
				}
				else if ( level < 0.5f )
				{
					outputBuffer[ y, x ] = Color.Lerp( shoreWaterColor, legend[ 1 ], 4.0f * ( level - 0.25f ) );
				}
				else if ( level < 1.0f )
				{
					outputBuffer[ y, x ] = legend[ 1 ];
				}
				else
				{
					level = ( level - 1.0f ) * ( legend.Length - 1 ) / ( legend.Length ) + 1.0f;

					if ( level >= maxElevation )
					{
						outputBuffer[ y, x ] = legend[ legend.Length - 1 ];
					}
					else
					{
						var levelA = Mathf.FloorToInt( level );
						var levelB = levelA + 1;

						var colorA = legend[ levelA ];
						var colorB = legend[ levelB ];

						var t = level - levelA;

						outputBuffer[ y, x ] = Color.Lerp( colorA, colorB, t );
					}
				}

				outputBuffer[ y, x ].a = m_buffer[ y, x ];
			}
		} );

		UnityEngine.Debug.Log( "Output Buffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// return the processed buffer
		return outputBuffer;
	}
}
