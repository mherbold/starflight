
using UnityEngine;
using System.Diagnostics;

public class ScaleToPowerOfTwo
{
	readonly float[,] m_buffer;

	readonly int m_width;
	readonly int m_height;

	public ScaleToPowerOfTwo( float[,] buffer )
	{
		m_buffer = buffer;

		m_width = buffer.GetLength( 1 );
		m_height = buffer.GetLength( 0 );
	}

	public float[,] Process( int xScale, int yScale )
	{
		UnityEngine.Debug.Log( "*** Scale to Power of Two Process ***" );

		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var height = GetNextPowerOfTwo( m_height );
		var width = GetNextPowerOfTwo( m_width );

		var outputBuffer = new float[ height, width ];

		// pad top and bottom to adjust height

		var topLevel = m_buffer[ 0, 0 ];
		var bottomLevel = m_buffer[ m_height - 1, 0 ];
		var numRowsToPad = ( height - m_height ) / 2;

		for ( var y = 0; y < numRowsToPad; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				outputBuffer[ y, x ] = topLevel;
				outputBuffer[ height - y - 1, x ] = bottomLevel;
			}
		}

		// adjust width by duplicating columns at carefully selected intervals

		var halfScaleX = xScale / 2;
		var numColumnsToPad = ( width - m_width );
		var columnsDuplicated = 0;
		var x2 = 0;

		for ( var x = 0; x < width; x++ )
		{
			for ( var y = 0; y < m_height; y++ )
			{
				outputBuffer[ y + numRowsToPad, x ] = m_buffer[ y, x2 ];
			}

			var numColumnsToBeDuplicatedByNow = Mathf.RoundToInt( (float) x / (float) width * (float) numColumnsToPad );

			bool isOnDuplicationInterval = ( ( ( x2 - halfScaleX ) % xScale ) == 0 );

			if ( ( columnsDuplicated < numColumnsToBeDuplicatedByNow ) && isOnDuplicationInterval )
			{
				columnsDuplicated++;
				
				//for ( var y = 0; y < m_height; y++ )
				//{
				//	outputBuffer[ y + numRowsToPad, x ] = 1.0f;
				//}
			}
			else
			{
				x2++;
			}
		}

		UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// return the processed buffer

		return outputBuffer;
	}

	int GetNextPowerOfTwo( int x )
	{
		var powerOfTwo = 2;

		x--;

		while ( ( x >>= 1 ) != 0 )
		{
			powerOfTwo <<= 1;
		}

		return powerOfTwo;
	}
}
