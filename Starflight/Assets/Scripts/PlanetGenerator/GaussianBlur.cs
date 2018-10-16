
using UnityEngine;

using System.Diagnostics;
using System.Threading.Tasks;

public class GaussianBlur
{
	readonly float[,] m_buffer;

	readonly int m_width;
	readonly int m_height;

	public GaussianBlur( float[,] buffer )
	{
		m_buffer = buffer;

		m_width = buffer.GetLength( 1 );
		m_height = buffer.GetLength( 0 );
	}

	public float[,] Process( int xBlurRadius, int yBlurRadius )
	{
		UnityEngine.Debug.Log( "*** Gaussian Blur Process ***" );

		var stopwatch = new Stopwatch();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var xBlurBuffer = new float[ m_height, m_width ];
		var yBlurBuffer = new float[ m_height, m_width ];

		// compute blur kernel

		var kernelWidth = xBlurRadius * 2 + 1;

		var kernel = new float[ kernelWidth ];
		var scale = 0.0f;

		for ( var i = 0; i < kernelWidth; i++ )
		{
			scale += kernel[ i ] = 1.0f * Mathf.Exp( -Mathf.Pow( i - xBlurRadius, 2.0f ) / ( 2.0f * Mathf.Pow( xBlurRadius / 3.2f, 2.0f ) ) );
		}

		scale = 1.0f / scale;

		// blur X

		stopwatch.Start();

		var steps = 16;
		var stepSize = m_height / steps;

		Parallel.For( 0, steps, parallelOptions, step =>
		{
			var y1 = step * stepSize;
			var y2 = y1 + stepSize;

			for ( var y = y1; y < y2; y++ )
			{
				for ( var x = 0; x < m_width; x++ )
				{
					for ( var i = 0; i < kernelWidth; i++ )
					{
						var x2 = Mathf.RoundToInt( x + i - xBlurRadius );

						while ( x2 < 0 )
						{
							x2 += m_width;
						}

						while ( x2 >= m_width )
						{
							x2 -= m_width;
						}

						xBlurBuffer[ y, x ] += m_buffer[ y, x2 ] * kernel[ i ];
					}

					xBlurBuffer[ y, x ] *= scale;
				}
			}
		} );

		UnityEngine.Debug.Log( "Blur X - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// compute blur kernel

		kernelWidth = yBlurRadius * 2 + 1;

		kernel = new float[ kernelWidth ];
		scale = 0.0f;

		for ( var i = 0; i < kernelWidth; i++ )
		{
			scale += kernel[ i ] = 1.0f * Mathf.Exp( -Mathf.Pow( i - yBlurRadius, 2.0f ) / ( 2.0f * Mathf.Pow( yBlurRadius / 3.2f, 2.0f ) ) );
		}

		scale = 1.0f / scale;

		// blur Y

		stopwatch.Restart();

		stepSize = m_width / steps;

		Parallel.For( 0, steps, parallelOptions, step =>
		{
			var x1 = step * stepSize;
			var x2 = x1 + stepSize;

			for ( var x = x1; x < x2; x++ )
			{
				for ( var y = 0; y < m_height; y++ )
				{
					for ( var i = 0; i < kernelWidth; i++ )
					{
						var y2 = Mathf.RoundToInt( y + i - yBlurRadius );

						if ( y2 < 0 )
						{
							y2 = 0;
						}

						if ( y2 >= m_height )
						{
							y2 = ( m_height - 1 );
						}

						yBlurBuffer[ y, x ] += xBlurBuffer[ y2, x ] * kernel[ i ];
					}

					yBlurBuffer[ y, x ] *= scale;
				}
			}
		} );

		UnityEngine.Debug.Log( "Blur Y - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// return the processed buffer

		return yBlurBuffer;
	}
}
