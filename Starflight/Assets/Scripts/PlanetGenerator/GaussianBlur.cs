
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

	public float[,] Process( int landBlurRadius, int waterBlurRadius )
	{
		UnityEngine.Debug.Log( "*** Gaussian Blur Process ***" );

		var stopwatch = new Stopwatch();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var xBlurBuffer = new float[ m_height, m_width ];
		var yBlurBuffer = new float[ m_height, m_width ];

		// initialize buffer with original height

		for ( var y = 0; y < m_height; y++ )
		{
			for ( var x = 0; x < m_width; x++ )
			{
				yBlurBuffer[ y, x ] = m_buffer[ y, x ];
			}
		}
		
		UnityEngine.Debug.Log( "Initialize - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// blur land

		if ( landBlurRadius > 0 )
		{
			// compute blur kernel

			var kernelWidth = landBlurRadius * 2 + 1;

			var kernel = new float[ kernelWidth ];
			var scale = 0.0f;

			for ( var i = 0; i < kernelWidth; i++ )
			{
				scale += kernel[ i ] = 1.0f * Mathf.Exp( -Mathf.Pow( i - landBlurRadius, 2.0f ) / ( 2.0f * Mathf.Pow( landBlurRadius / 3.2f, 2.0f ) ) );
			}

			scale = 1.0f / scale;

			// land blur X

			stopwatch.Start();

			Parallel.For( 0, m_height, parallelOptions, y =>
			{
				for ( var x = 0; x < m_width; x++ )
				{
					xBlurBuffer[ y, x ] = 0.0f;

					for ( var i = 0; i < kernelWidth; i++ )
					{
						var x2 = x + i - landBlurRadius;

						while ( x2 < 0 )
						{
							x2 += m_width;
						}

						while ( x2 >= m_width )
						{
							x2 -= m_width;
						}

						xBlurBuffer[ y, x ] += yBlurBuffer[ y, x2 ] * kernel[ i ];
					}

					xBlurBuffer[ y, x ] *= scale;
				}
			} );

			UnityEngine.Debug.Log( "Land Blur X - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

			stopwatch.Restart();

			// land blur Y

			Parallel.For( 0, m_width, parallelOptions, x =>
			{
				for ( var y = 0; y < m_height; y++ )
				{
					yBlurBuffer[ y, x ] = 0.0f;

					for ( var i = 0; i < kernelWidth; i++ )
					{
						var y2 = y + i - landBlurRadius;

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
			} );

			UnityEngine.Debug.Log( "Land Blur Y - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

			stopwatch.Restart();
		}

		// blur water

		if ( waterBlurRadius > 0 )
		{
			// compute blur kernel

			var kernelWidth = waterBlurRadius * 2 + 1;

			var kernel = new float[ kernelWidth ];
			var scale = 0.0f;

			for ( var i = 0; i < kernelWidth; i++ )
			{
				scale += kernel[ i ] = 1.0f * Mathf.Exp( -Mathf.Pow( i - waterBlurRadius, 2.0f ) / ( 2.0f * Mathf.Pow( waterBlurRadius / 3.2f, 2.0f ) ) );
			}

			scale = 1.0f / scale;

			// put the water level back in unblurred

			for ( var y = 0; y < m_height; y++ )
			{
				for ( var x = 0; x < m_width; x++ )
				{
					if ( m_buffer[ y, x ] == 0.0f )
					{
						yBlurBuffer[ y, x ] = 0.0f;
					}
				}
			}

			UnityEngine.Debug.Log( "Water Level Initialize - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

			stopwatch.Restart();

			// water blur X

			Parallel.For( 0, m_height, parallelOptions, y =>
			{
				for ( var x = 0; x < m_width; x++ )
				{
					xBlurBuffer[ y, x ] = 0.0f;

					for ( var i = 0; i < kernelWidth; i++ )
					{
						var x2 = x + i - waterBlurRadius;

						while ( x2 < 0 )
						{
							x2 += m_width;
						}

						while ( x2 >= m_width )
						{
							x2 -= m_width;
						}

						xBlurBuffer[ y, x ] += yBlurBuffer[ y, x2 ] * kernel[ i ];
					}

					xBlurBuffer[ y, x ] *= scale;
				}
			} );

			UnityEngine.Debug.Log( "Water Blur X - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

			stopwatch.Restart();

			// water blur Y

			Parallel.For( 0, m_width, parallelOptions, x =>
			{
				for ( var y = 0; y < m_height; y++ )
				{
					yBlurBuffer[ y, x ] = 0.0f;

					for ( var i = 0; i < kernelWidth; i++ )
					{
						var y2 = y + i - waterBlurRadius;

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
			} );

			UnityEngine.Debug.Log( "Water Blur Y - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

			stopwatch.Restart();
		}
		
		// return the processed buffer

		return yBlurBuffer;
	}
}
